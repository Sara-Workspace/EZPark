using DAL;

namespace DAL.Repositories
{
    public interface IReservationsRepository
    {
        List<Reservations> GetAllReservations();
        Reservations? GetReservationById(int id);
        void AddReservation(Reservations reservation);
        void UpdateReservation(Reservations reservation);
        void DeleteReservation(Reservations reservation);
        List<Reservations> GetReservationsByDriverId(int driverId);
        List<Reservations> GetReservationsBySpotId(int spotId);
        List<Reservations> GetReservationsByDateRange(DateTime from, DateTime to);
    }
}