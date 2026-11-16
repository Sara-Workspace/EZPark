using DTO;

namespace BLL.Interfaces
{
    public interface IReservationsService
    {
        List<ReservationDTO> GetAllReservations();
        ReservationDTO GetReservationById(int id);                                  // זורק NotFoundException אם לא נמצאה
        ReservationDTO CreateReservation(ReservationCreateDTO dto, int driverId);
        List<ReservationDTO> GetReservationsByDriverId(int driverId);
        ReservationDTO StartParking(int spotId, string userEmail);
        ReservationDTO CompleteParking(int reservationId, string userEmail);
        List<ReservationDTO> GetMyReservations(string userEmail);
    }
}