using DTO;

namespace BLL.Interfaces
{
    public interface IParkingSpotService
    {
        List<ParkingSpotDTO> GetAllParkingSpots();
        ParkingSpotDTO GetParkingSpotById(int id);                                   // זורק NotFoundException אם לא נמצאה

        // async: מחכה ל-API החיצוני (OpenStreetMap) כדי למצוא את המיקום לפי הכתובת.
        // זורק BusinessRuleException אם הכתובת לא נמצאה
        Task<ParkingSpotDTO> CreateParkingSpotAsync(ParkingSpotCreateDTO dto, int ownerId, CancellationToken ct = default);

        ParkingSpotDTO UpdateParkingSpot(int id, ParkingSpotDTO dto, int ownerId, bool isAdmin);   // זורק NotFoundException אם לא נמצאה / לא שלך
        void DeleteParkingSpot(int id, int ownerId, bool isAdmin);                                 // זורק NotFoundException אם לא נמצאה / לא שלך
        List<ParkingSpotDTO> GetParkingSpotsByOwnerId(int ownerId);
    }
}