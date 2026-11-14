using DTO;

namespace BLL.Interfaces
{
    public interface IReservationsService
    {
        List<ReservationDTO> GetAllReservations();
        ReservationDTO GetReservationById(int id);                                  // זורק NotFoundException אם לא נמצאה
        ReservationDTO CreateReservation(ReservationCreateDTO dto, int driverId);
        List<ReservationDTO> GetReservationsByDriverId(int driverId);
    }
}