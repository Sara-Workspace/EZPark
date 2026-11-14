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
        private readonly IMapper _mapper;

        public ReservationsService(IReservationsRepository reservationsRepo, IMapper mapper)
        {
            _reservationsRepo = reservationsRepo;
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