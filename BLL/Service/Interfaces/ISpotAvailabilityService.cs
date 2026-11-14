using DTO;

namespace BLL.Interfaces
{
    public interface ISpotAvailabilityService
    {
        List<SpotAvailabilityDTO> GetAllSpotAvailabilities();
        SpotAvailabilityDTO GetSpotAvailabilityById(int id);                                  // זורק NotFoundException אם לא נמצאה
        SpotAvailabilityDTO CreateSpotAvailability(SpotAvailabilityCreateDTO dto, int ownerId); // זורק NotFoundException אם החניה לא קיימת / לא שלו
        SpotAvailabilityDTO UpdateSpotAvailability(int id, SpotAvailabilityDTO dto);          // זורק NotFoundException אם לא נמצאה
        void DeleteSpotAvailability(int id, int ownerId);                                     // זורק NotFoundException אם לא נמצאה / לא שלו
        List<SpotAvailabilityDTO> GetAvailabilityBySpotId(int spotId);
    }
}