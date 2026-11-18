using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Payments;
using DAL;
using DAL.Repositories;
using DTO.Wallet;

namespace BLL.Services
{
    public class WalletService : IWalletService
    {
        // 💰 העמלה של האתר: 10%. בעל החניה מקבל את השאר (90%). רוצים לשנות? משנים רק כאן
        public const decimal PlatformFeeRate = 0.10m;

        private readonly IWalletRepository _walletRepo;
        private readonly IPayPalService _payPal;

        public WalletService(IWalletRepository walletRepo, IPayPalService payPal)
        {
            _walletRepo = walletRepo;
            _payPal = payPal;
        }

        private Users GetUser(string email)
        {
            return _walletRepo.GetUserByEmail(email)
                ?? throw new NotFoundException("המשתמש לא נמצא במערכת.");
        }

        private WalletDTO BuildWallet(Users user)
        {
            return new WalletDTO
            {
                Balance = user.Balance,
                Transactions = _walletRepo.GetTransactionsByUserId(user.UserId, 20)
                    .Select(t => new WalletTransactionDTO
                    {
                        Type = t.Type,
                        Amount = t.Amount,
                        Description = t.Description,
                        CreatedAt = t.CreatedAt
                    })
                    .ToList()
            };
        }

        public WalletDTO GetMyWallet(string email)
        {
            return BuildWallet(GetUser(email));
        }

        // ===== טעינת ארנק, שלב 1: פותחים הזמנה ב-PayPal =====
        public async Task<TopUpOrderDTO> StartTopUpAsync(string email, decimal amount, CancellationToken ct = default)
        {
            var user = GetUser(email);
            var order = await _payPal.CreateOrderAsync(amount, ct);

            // רושמים "טעינה ממתינה": ככה בשלב 2 נדע שההזמנה הזאת שייכת למשתמש הזה ועל כמה היא
            _walletRepo.AddTransaction(new WalletTransaction
            {
                UserId = user.UserId,
                Type = WalletTransactionTypes.TopUp,
                Amount = amount,
                PayPalOrderId = order.OrderId,
                Status = "Pending",
                Description = "טעינת ארנק ב-PayPal"
            });

            // הדפדפן יעבור לכתובת הזאת, והמשתמש ישלם בדף של PayPal
            return new TopUpOrderDTO { OrderId = order.OrderId, ApproveUrl = order.ApproveUrl };
        }

        // ===== טעינת ארנק, שלב 2: אחרי שהמשתמש שילם, השרת גובה מ-PayPal ומוסיף ליתרה =====
        public async Task<WalletDTO> CompleteTopUpAsync(string email, string orderId, CancellationToken ct = default)
        {
            var user = GetUser(email);

            // ההזמנה חייבת להיות כזו שפתחנו בשלב 1, ולמשתמש הזה
            var pending = _walletRepo.GetByPayPalOrderId(orderId);
            if (pending == null || pending.UserId != user.UserId)
                throw new NotFoundException("הטעינה לא נמצאה.");

            // כבר הוספנו את הכסף הזה? לא מוסיפים פעמיים (למשל אם לחצו פעמיים)
            if (pending.Status == "Completed")
                return BuildWallet(user);

            // הבדיקה האמיתית: PayPal עונה כמה באמת נגבה
            var paid = await _payPal.CaptureOrderAsync(orderId, ct);
            if (paid != pending.Amount)
                throw new BusinessRuleException("הסכום ששולם לא תואם לסכום הטעינה.");

            user.Balance += paid;
            pending.Status = "Completed";
            pending.CreatedAt = DateTime.UtcNow;
            _walletRepo.UpdateTransaction(pending);   // השמירה כוללת גם את היתרה החדשה

            return BuildWallet(user);
        }

        // ===== משיכה (בעל חניה): מוציאים כסף מהארנק =====
        // בפרויקט זו הדמיה: הכסף יורד מהיתרה ונרשם. בעולם האמיתי כאן היינו שולחים אותו ב-PayPal Payouts
        public WalletDTO Withdraw(string email, decimal amount)
        {
            var user = GetUser(email);
            if (amount > user.Balance)
                throw new BusinessRuleException($"אין מספיק כסף בארנק. היתרה שלך: {user.Balance:0.00} ₪.");

            user.Balance -= amount;
            _walletRepo.AddTransaction(new WalletTransaction   // השמירה כוללת גם את היתרה החדשה
            {
                UserId = user.UserId,
                Type = WalletTransactionTypes.Withdrawal,
                Amount = -amount,
                Description = "משיכה לחשבון"
            });

            return BuildWallet(user);
        }

        public PlatformSummaryDTO GetPlatformSummary()
        {
            return new PlatformSummaryDTO
            {
                TotalFees = _walletRepo.GetPlatformTotal(WalletTransactionTypes.PlatformFee),
                ParkingCount = _walletRepo.CountPlatform(WalletTransactionTypes.PlatformFee),
                TotalFines = _walletRepo.GetPlatformTotal(WalletTransactionTypes.Fine)
            };
        }

        // ===== תשלום על חניה: הנהג משלם 100%, בעל החניה מקבל 90%, האתר 10% =====
        // הנהג רואה רק כמה שילם. החלוקה נרשמת בשורות נפרדות
        public void PayForParking(Users driverUser, Users? ownerUser, decimal cost, int reservationId, string spotName)
        {
            decimal platformFee = Math.Round(cost * PlatformFeeRate, 2, MidpointRounding.AwayFromZero);
            decimal ownerShare = cost - platformFee;   // ככה הסכומים תמיד מסתכמים בדיוק ל-100%

            // כל השורות של התשלום הזה. נשמרות ביחד בסוף (או הכל או כלום)
            var rows = new List<WalletTransaction>();

            // 1. מהנהג (יכול לרדת למינוס אם חנה יותר זמן ממה שהיה לו. אז לא יוכל לחנות שוב עד שיטען)
            driverUser.Balance -= cost;
            rows.Add(new WalletTransaction
            {
                UserId = driverUser.UserId,
                Type = WalletTransactionTypes.Parking,
                Amount = -cost,
                ReservationId = reservationId,
                Description = $"חניה ב{spotName}"
            });

            // 2. לבעל החניה: 90%
            if (ownerUser != null)
            {
                ownerUser.Balance += ownerShare;
                rows.Add(new WalletTransaction
                {
                    UserId = ownerUser.UserId,
                    Type = WalletTransactionTypes.Earning,
                    Amount = ownerShare,
                    ReservationId = reservationId,
                    Description = $"נהג חנה ב{spotName}"
                });
            }

            // 3. לאתר: 10% (שורה בלי משתמש). אם אין בעל חניה - הכל לאתר
            rows.Add(new WalletTransaction
            {
                UserId = null,
                Type = WalletTransactionTypes.PlatformFee,
                Amount = ownerUser != null ? platformFee : cost,
                ReservationId = reservationId,
                Description = $"עמלת האתר על חניה ב{spotName}"
            });

            // שמירה אחת: השורות + היתרות של הנהג ובעל החניה
            _walletRepo.AddTransactions(rows);
        }

        // ===== קנס: יורד מהנהג מיד, ועובר לאתר =====
        public void ChargeFine(Users driverUser, decimal amount, string spotName)
        {
            driverUser.Balance -= amount;
            _walletRepo.AddTransactions(new List<WalletTransaction>
            {
                new WalletTransaction
                {
                    UserId = driverUser.UserId,
                    Type = WalletTransactionTypes.Fine,
                    Amount = -amount,
                    Description = $"קנס: חניה ב{spotName} בלי הזמנה"
                },
                new WalletTransaction
                {
                    UserId = null,
                    Type = WalletTransactionTypes.Fine,
                    Amount = amount,
                    Description = $"קנס על חניה ב{spotName}"
                }
            });
        }
    }
}
