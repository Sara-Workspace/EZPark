using DTO;

namespace BLL.Interfaces
{
    public interface IParkingSpotService
    {
        List<ParkingSpotDTO> GetAllParkingSpots();
        ParkingSpotDTO GetParkingSpotById(int id);                                   // זורק NotFoundException אם לא נמצאה
        ParkingSpotDTO CreateParkingSpot(ParkingSpotCreateDTO dto, int ownerId);
        ParkingSpotDTO UpdateParkingSpot(int id, ParkingSpotDTO dto, int ownerId);   // זורק NotFoundException אם לא נמצאה / לא שלך
        void DeleteParkingSpot(int id, int ownerId);                                 // זורק NotFoundException אם לא נמצאה / לא שלך
        List<ParkingSpotDTO> GetParkingSpotsByOwnerId(int ownerId);
    }
}