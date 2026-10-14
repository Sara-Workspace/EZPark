using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using DTO;

namespace BLL.Repositories
{
    public class ReservationsRepository : IReservationsRepository
    {
        private readonly ParkingDbContext _context;
        private readonly IMapper _mapper;

        public ReservationsRepository(ParkingDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<ReservationDTO> GetAllReservations()
        {
            return _context.Reservations
                .ProjectTo<ReservationDTO>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public ReservationDTO? GetReservationById(int id)
        {
            return _context.Reservations
                .Where(r => r.ReservationID == id)
                .ProjectTo<ReservationDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefault();
        }

        public ReservationDTO CreateReservation(ReservationCreateDTO dto)
        {
            var reservation = _mapper.Map<Reservations>(dto);
            reservation.CreatedAt = DateTime.Now;
            reservation.Status = "Pending";             // סטטוס התחלתי: ממתינה
            reservation.TotalPrice = 0;                 // המחיר יחושב בהמשך בשכבת Services

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            return _mapper.Map<ReservationDTO>(reservation);
        }

        public ReservationDTO? UpdateReservation(int id, ReservationDTO dto)
        {
            var reservation = _context.Reservations.Find(id);
            if (reservation == null) return null;

            _mapper.Map(dto, reservation);
            _context.SaveChanges();

            return _mapper.Map<ReservationDTO>(reservation);
        }

        public bool DeleteReservation(int id)
        {
            var reservation = _context.Reservations.Find(id);
            if (reservation == null) return false;

            _context.Reservations.Remove(reservation);
            _context.SaveChanges();
            return true;
        }

        public List<ReservationDTO> GetReservationsByDriverId(int driverId)
        {
            return _context.Reservations
                .Where(r => r.DriverID == driverId)
                .ProjectTo<ReservationDTO>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public List<ReservationDTO> GetReservationsBySpotId(int spotId)
        {
            return _context.Reservations
                .Where(r => r.SpotID == spotId)
                .ProjectTo<ReservationDTO>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public List<ReservationDTO> GetReservationsByDateRange(DateTime from, DateTime to)
        {
            return _context.Reservations
                .Where(r => r.StartTime < to && r.EndTime > from)   // הזמנה שחופפת לטווח: מתחילה לפני סופו ונגמרת אחרי תחילתו
                .ProjectTo<ReservationDTO>(_mapper.ConfigurationProvider)
                .ToList();
        }
    }
}