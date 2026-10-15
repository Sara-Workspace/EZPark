using DAL;

namespace DAL.Repositories
{
    public class ReservationsRepository : IReservationsRepository
    {
        private readonly ParkingDbContext _context;

        public ReservationsRepository(ParkingDbContext context)
        {
            _context = context;
        }

        public List<Reservations> GetAllReservations()
        {
            return _context.Reservations.ToList();
        }

        public Reservations? GetReservationById(int id)
        {
            return _context.Reservations.Find(id);
        }

        public void AddReservation(Reservations reservation)
        {
            _context.Reservations.Add(reservation);
            _context.SaveChanges();
        }

        public void UpdateReservation(Reservations reservation)
        {
            _context.Reservations.Update(reservation);
            _context.SaveChanges();
        }

        public void DeleteReservation(Reservations reservation)
        {
            _context.Reservations.Remove(reservation);
            _context.SaveChanges();
        }

        public List<Reservations> GetReservationsByDriverId(int driverId)
        {
            return _context.Reservations
                .Where(r => r.DriverID == driverId)
                .ToList();
        }

        public List<Reservations> GetReservationsBySpotId(int spotId)
        {
            return _context.Reservations
                .Where(r => r.SpotID == spotId)
                .ToList();
        }

        public List<Reservations> GetReservationsByDateRange(DateTime from, DateTime to)
        {
            return _context.Reservations
                .Where(r => r.StartTime < to && r.EndTime > from)
                .ToList();
        }
    }
}