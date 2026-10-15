using DAL;

namespace DAL.Repositories
{
    public interface ISpotAvailabilityRepository
    {
        List<SpotAvailability> GetAllSpotAvailabilities();
        SpotAvailability? GetSpotAvailabilityById(int id);
        void AddSpotAvailability(SpotAvailability availability);
        void UpdateSpotAvailability(SpotAvailability availability);
        void DeleteSpotAvailability(SpotAvailability availability);
        List<SpotAvailability> GetAvailabilityBySpotId(int spotId);
    }
}