using AutoMapper;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL;
using DAL.Repositories;
using DTO;

namespace BLL.Services
{
    public class ReservationsService : IReservationsService
    {
        private readonly IReservationsRepository _reservationsRepo;
        private readonly IParkingSpotRepository _spotRepo;   // חדש: כדי לבדוק שהחניה קיימת ופעילה
        private readonly IDriverRepository _driverRepo;
        private readonly IOwnersRepository _ownerRepo;
        private readonly SpotHoldRegistry _holdRegistry;
        private readonly ISpotOpenHoursService _openHours; // שעות הזמינות שבעל החניה קבע
        private readonly IWalletService _wallet;           // 👛 הארנקים: תשלום על החניה (90% / 10%)
        private readonly IMapper _mapper;

        public ReservationsService(
            IReservationsRepository reservationsRepo,
            IParkingSpotRepository spotRepo,
            IDriverRepository driverRepo,
            IOwnersRepository ownerRepo,
            SpotHoldRegistry holdRegistry,
            ISpotOpenHoursService openHours,
            IWalletService wallet,
            IMapper mapper)
        {
            _reservationsRepo = reservationsRepo;
            _spotRepo = spotRepo;
            _driverRepo = driverRepo;
            _ownerRepo = ownerRepo;
            _holdRegistry = holdRegistry;
            _openHours = openHours;
            _wallet = wallet;
            _mapper = mapper;
        }

        public List<ReservationDTO> GetAllReservations()
        {
            var reservations = _reservationsRepo.GetAllReservations();
            return _mapper.Map<List<ReservationDTO>>(reservations);
        }

        public ReservationDTO GetReservationById(int id)
        {
            // אם ההזמנה לא קיימת - זורקים שגיאה, והשומר (GlobalExceptionFilter) מחזיר 404
            var reservation = _reservationsRepo.GetReservationById(id)
                ?? throw new NotFoundException($"הזמנה מספר {id} לא נמצאה.");

            return _mapper.Map<ReservationDTO>(reservation);
        }

        // לוגיקה עסקית: הגדרת תאריך יצירה, סטטוס ממתין ואתחול מחיר סופי לפני חישוב
        // הנהג מגיע כפרמטר (מהטוקן), לא מה-DTO
        public ReservationDTO CreateReservation(ReservationCreateDTO dto, int driverId)
        {
            // ===== כללים עסקיים: נבדקים לפני שיוצרים משהו =====

            // 1. סיום אחרי התחלה
            if (dto.EndTime <= dto.StartTime)
                throw new BusinessRuleException("שעת הסיום חייבת להיות אחרי שעת ההתחלה.");

            // 2. לא מזמינים לזמן שכבר עבר
            //    (משווים בשעון אחיד - UTC - כי Swagger ו-React שולחים זמנים עם Z בסוף)
            if (dto.StartTime.ToUniversalTime() < DateTime.UtcNow)
                throw new BusinessRuleException("אי אפשר להזמין חניה לזמן שכבר עבר.");

            // 3. החניה שנבחרה קיימת (כמו "הקטגוריה שנבחרה" במדריך) - 400 ולא 404
            var spot = _spotRepo.GetParkingSpotById(dto.SpotID)
                ?? throw new BusinessRuleException("החניה שנבחרה לא קיימת.");

            // 4. החניה פעילה
            if (!spot.IsActive)
                throw new BusinessRuleException("החניה שנבחרה לא פעילה כרגע.");

            // ===== הכל תקין - יוצרים את ההזמנה =====
            var reservation = _mapper.Map<Reservations>(dto);
            reservation.DriverID = driverId;
            reservation.CreatedAt = DateTime.Now;
            reservation.Status = "Pending";
            reservation.TotalPrice = 0; // יחושב בהמשך בלוגיקה מתקדמת

            _reservationsRepo.AddReservation(reservation);
            return _mapper.Map<ReservationDTO>(reservation);
        }

        public List<ReservationDTO> GetReservationsByDriverId(int driverId)
        {
            var reservations = _reservationsRepo.GetReservationsByDriverId(driverId);
            return _mapper.Map<List<ReservationDTO>>(reservations);
        }
        public List<ReservationDTO> GetMyReservations(string userEmail)
        {
            var driver = _driverRepo.GetDriverByEmail(userEmail);
            if (driver == null) return new List<ReservationDTO>();

            var reservations = _reservationsRepo.GetReservationsByDriverId(driver.DriverID);
            return _mapper.Map<List<ReservationDTO>>(reservations);
        }

        // לחיצה על "חניתי" - פתיחת הזמנה פעילה והתחלת מדידת זמן
        public ReservationDTO StartParking(int spotId, string userEmail)
        {
            var driver = _driverRepo.GetDriverByEmail(userEmail)
                ?? throw new BusinessRuleException("יש להזין מספר רישוי לפני תחילת חניה.");

            if (string.IsNullOrWhiteSpace(driver.LicensePlate))
                throw new BusinessRuleException("יש להזין מספר רישוי לפני תחילת חניה.");

            // אין כסף בארנק (או שהוא במינוס) - קודם טוענים
            if (driver.User.Balance <= 0)
                throw new BusinessRuleException("אין מספיק כסף בארנק. טענו את הארנק כדי לחנות.");

            // נהג יכול לחנות במקום אחד בלבד בכל רגע
            if (driver.Reservations.Any(r => r.Status == "Active"))
                throw new BusinessRuleException("יש לך כבר חניה פעילה. יש לסיים אותה לפני תחילת חניה חדשה.");

            var spot = _spotRepo.GetParkingSpotById(spotId)
                ?? throw new NotFoundException($"חניה מספר {spotId} לא נמצאה.");


            if (!spot.IsActive)
                throw new BusinessRuleException("החניה אינה זמינה כרגע.");

            // "חניתי" מותר רק בשעות שבעל החניה קבע שהחניה זמינה
            if (!_openHours.IsOpenNow(spotId))
                throw new BusinessRuleException("החניה לא זמינה בשעה הזאת לפי השעות שבעל החניה קבע.");

            // מוודאים שהחניה לא שמורה כרגע למישהו אחר
            if (_holdRegistry.IsHeldByAnother(spotId, driver.DriverID))
                throw new ConflictException("החניה שמורה כרגע לנהג אחר.");

            var nowUtc = DateTime.UtcNow;
            var reservation = new Reservations
            {
                SpotID = spotId,
                ParkingSpot = spot,
                DriverID = driver.DriverID,
                StartTime = nowUtc,
                EndTime = nowUtc, // יעודכן ברגע שילחץ "סיום חניה"
                Status = "Active",
                CreatedAt = nowUtc,
                TotalPrice = 0
            };

            _reservationsRepo.AddReservation(reservation);

            // משחררים את השמירה הזמנית בזיכרון ומסמנים את החניה כתפוסה ב-DB
            _holdRegistry.Release(spotId, driver.DriverID);
            spot.IsActive = false;
            _spotRepo.UpdateParkingSpot(spot);

            return _mapper.Map<ReservationDTO>(reservation);
        }

        // לחיצה על "סיום חניה" - חישוב זמן ותשלום מהארנק: 90% לבעל החניה, 10% לאתר
        public ReservationDTO CompleteParking(int reservationId, string userEmail)
        {
            var driver = _driverRepo.GetDriverByEmail(userEmail)
                ?? throw new NotFoundException("פרופיל נהג לא נמצא.");

            var reservation = _reservationsRepo.GetReservationById(reservationId)
                ?? throw new NotFoundException($"הזמנה מספר {reservationId} לא נמצאה.");

            if (reservation.DriverID != driver.DriverID)
                throw new NotFoundException($"הזמנה מספר {reservationId} לא נמצאה.");

            if (reservation.Status != "Active")
                throw new BusinessRuleException("הזמנה זו כבר הסתיימה או אינה פעילה.");

            var spot = _spotRepo.GetParkingSpotById(reservation.SpotID)
                ?? throw new NotFoundException("החניה לא נמצאה.");

            var endTimeUtc = DateTime.UtcNow;
            int totalMinutes = Math.Max(1, (int)Math.Ceiling((endTimeUtc - reservation.StartTime).TotalMinutes));

            // חישוב העלות בשקלים: תעריף לשעה מחולק ב-60, כפול דקות החניה (מינימום 1 ₪), מעוגל לאגורות
            decimal pricePerMinute = spot.BasePricePerHour / 60m;
            decimal cost = Math.Max(1m, Math.Round(totalMinutes * pricePerMinute, 2, MidpointRounding.AwayFromZero));

            // 1. התשלום מהארנק: הנהג משלם הכל, בעל החניה מקבל 90%, האתר 10% (הכל ב-WalletService)
            var owner = _ownerRepo.GetOwnerById(spot.OwnerId);
            _wallet.PayForParking(driver.User, owner?.User, cost, reservation.ReservationID, spot.Address);

            // 2. סגירת ההזמנה והחזרת החניה למצב פנוי (IsActive = true)
            reservation.EndTime = endTimeUtc;
            reservation.TotalPrice = cost;
            reservation.Status = "Completed";
            _reservationsRepo.UpdateReservation(reservation);

            spot.IsActive = true;
            _spotRepo.UpdateParkingSpot(spot);

            return _mapper.Map<ReservationDTO>(reservation);
        }
    }
}