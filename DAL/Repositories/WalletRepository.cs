using DAL;

namespace DAL.Repositories
{
    // כמו שאר ה-Repositories: כל פעולה שמשנה משהו גם שומרת (SaveChanges).
    // SaveChanges שומר את כל מה שהשתנה, כולל היתרה (Balance) של המשתמשים שהשירות שינה לפני כן
    public class WalletRepository : IWalletRepository
    {
        private readonly ParkingDbContext _context;

        public WalletRepository(ParkingDbContext context)
        {
            _context = context;
        }

        public Users? GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        // התנועות האחרונות של משתמש, מהחדשה לישנה. רק אלה שבוצעו (לא טעינות שלא שולמו)
        public List<WalletTransaction> GetTransactionsByUserId(int userId, int take)
        {
            return _context.WalletTransactions
                .Where(t => t.UserId == userId && t.Status == "Completed")
                .OrderByDescending(t => t.CreatedAt)
                .Take(take)
                .ToList();
        }

        public WalletTransaction? GetByPayPalOrderId(string orderId)
        {
            return _context.WalletTransactions.FirstOrDefault(t => t.PayPalOrderId == orderId);
        }

        public void AddTransaction(WalletTransaction transaction)
        {
            _context.WalletTransactions.Add(transaction);
            _context.SaveChanges();
        }

        // כמה שורות ביחד ושמירה אחת: או שהכל נשמר, או ששום דבר לא נשמר.
        // (למשל בתשלום על חניה: לא יכול לקרות שמהנהג ירד כסף ובעל החניה לא קיבל)
        public void AddTransactions(List<WalletTransaction> transactions)
        {
            _context.WalletTransactions.AddRange(transactions);
            _context.SaveChanges();
        }

        public void UpdateTransaction(WalletTransaction transaction)
        {
            _context.WalletTransactions.Update(transaction);
            _context.SaveChanges();
        }

        // שורות של האתר = UserId ריק
        public decimal GetPlatformTotal(string type)
        {
            return _context.WalletTransactions
                .Where(t => t.UserId == null && t.Type == type && t.Status == "Completed")
                .Sum(t => (decimal?)t.Amount) ?? 0m;
        }

        public int CountPlatform(string type)
        {
            return _context.WalletTransactions
                .Count(t => t.UserId == null && t.Type == type && t.Status == "Completed");
        }
    }
}
