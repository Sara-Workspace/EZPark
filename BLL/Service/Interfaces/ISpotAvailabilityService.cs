using DTO;

namespace BLL.Interfaces
{
    public interface ISpotAvailabilityService
    {
        List<SpotAvailabilityDTO> GetAllSpotAvailabilities();
        SpotAvailabilityDTO? GetSpotAvailabilityById(int id);
        SpotAvailabilityDTO CreateSpotAvailability(SpotAvailabilityCreateDTO dto);
        SpotAvailabilityDTO? UpdateSpotAvailability(int id, SpotAvailabilityDTO dto);
        bool DeleteSpotAvailability(int id);
        List<SpotAvailabilityDTO> GetAvailabilityBySpotId(int spotId);
    }
}