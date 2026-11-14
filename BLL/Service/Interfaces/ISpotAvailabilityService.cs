using DTO;

namespace BLL.Interfaces
{
    public interface ISpotAvailabilityService
    {
        List<SpotAvailabilityDTO> GetAllSpotAvailabilities();
        SpotAvailabilityDTO? GetSpotAvailabilityById(int id);
        SpotAvailabilityDTO? CreateSpotAvailability(SpotAvailabilityCreateDTO dto, int ownerId);
        SpotAvailabilityDTO? UpdateSpotAvailability(int id, SpotAvailabilityDTO dto);
        bool DeleteSpotAvailability(int id, int ownerId);
        List<SpotAvailabilityDTO> GetAvailabilityBySpotId(int spotId);
    }
}
