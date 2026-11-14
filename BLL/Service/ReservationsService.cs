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
        private readonly IMapper _mapper;

        public ReservationsService(
            IReservationsRepository reservationsRepo,
            IParkingSpotRepository spotRepo,
            IMapper mapper)
        {
            _reservationsRepo = reservationsRepo;
            _spotRepo = spotRepo;
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
    }
}