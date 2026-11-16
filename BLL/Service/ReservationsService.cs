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
        private readonly IMapper _mapper;

        public ReservationsService(
            IReservationsRepository reservationsRepo,
            IParkingSpotRepository spotRepo,
            IDriverRepository driverRepo,
            IOwnersRepository ownerRepo,
            SpotHoldRegistry holdRegistry,
            IMapper mapper)
        {
            _reservationsRepo = reservationsRepo;
            _spotRepo = spotRepo;
            _driverRepo = driverRepo;
            _ownerRepo = ownerRepo;
            _holdRegistry = holdRegistry;
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

            RefreshMonthlyPointsIfNeeded(driver);

            if (driver.User.Points <= 0)
                throw new BusinessRuleException("נגמרו לך הנקודות למימוש לחודש זה.");

            // נהג יכול לחנות במקום אחד בלבד בכל רגע
            if (driver.Reservations.Any(r => r.Status == "Active"))
                throw new BusinessRuleException("יש לך כבר חניה פעילה. יש לסיים אותה לפני תחילת חניה חדשה.");

            var spot = _spotRepo.GetParkingSpotById(spotId)
                ?? throw new NotFoundException($"חניה מספר {spotId} לא נמצאה.");


            if (!spot.IsActive)
                throw new BusinessRuleException("החניה אינה זמינה כרגע.");

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

        // לחיצה על "סיום חניה" - חישוב זמן, הורדת נקודות, זיכוי משכיר + 10% בונוס
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

            // חישוב עלות בנקודות: תעריף לשעה מחולק ב-60 כפול דקות חניה (מינימום נקודה 1)
            decimal pricePerMinute = spot.BasePricePerHour / 60m;
            int pointsCost = Math.Max(1, (int)Math.Ceiling(totalMinutes * pricePerMinute));

            // 1. הורדת נקודות מהשוכר + קבלת 10% רווח/החזר בנקודות לפי האפיון
            int cashbackBonus = (int)Math.Round(pointsCost * 0.10m);
            driver.User.Points = Math.Max(0, driver.User.Points - pointsCost) + cashbackBonus;
            _driverRepo.UpdateDriver(driver);

            // 2. זיכוי בעל החניה בנקודות שהתקבלו
            var owner = _ownerRepo.GetOwnerById(spot.OwnerId);
            if (owner?.User != null)
            {
                owner.User.Points += pointsCost;
                _ownerRepo.UpdateOwner(owner);
            }

            // 3. סגירת ההזמנה והחזרת החניה למצב פנוי (IsActive = true)
            reservation.EndTime = endTimeUtc;
            reservation.TotalPrice = pointsCost;
            reservation.Status = "Completed";
            _reservationsRepo.UpdateReservation(reservation);

            spot.IsActive = true;
            _spotRepo.UpdateParkingSpot(spot);

            return _mapper.Map<ReservationDTO>(reservation);
        }

        // חידוש 3,000 נקודות אם עבר חודש מאז החידוש האחרון
        private void RefreshMonthlyPointsIfNeeded(Driver driver)
        {
            if (driver.User != null && driver.User.PointsResetAt.AddMonths(1) <= DateTime.UtcNow)
            {
                driver.User.Points = 3000;
                driver.User.PointsResetAt = DateTime.UtcNow;
                _driverRepo.UpdateDriver(driver);
            }
        }
    }
}