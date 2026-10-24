using DTO;

namespace BLL.Interfaces
{
    public interface IReservationsService
    {
        List<ReservationDTO> GetAllReservations();
        ReservationDTO? GetReservationById(int id);
        ReservationDTO CreateReservation(ReservationCreateDTO dto);
        List<ReservationDTO> GetReservationsByDriverId(int driverId);
    }
}