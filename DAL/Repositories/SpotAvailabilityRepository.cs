using DAL;

namespace DAL.Repositories
{
    public class SpotAvailabilityRepository : ISpotAvailabilityRepository
    {
        private readonly ParkingDbContext _context;

        public SpotAvailabilityRepository(ParkingDbContext context)
        {
            _context = context;
        }

        public List<SpotAvailability> GetAllSpotAvailabilities()
        {
            return _context.SpotAvailability.ToList();
        }

        public SpotAvailability? GetSpotAvailabilityById(int id)
        {
            return _context.SpotAvailability.Find(id);
        }

        public void AddSpotAvailability(SpotAvailability availability)
        {
            _context.SpotAvailability.Add(availability);
            _context.SaveChanges();
        }

        public void UpdateSpotAvailability(SpotAvailability availability)
        {
            _context.SpotAvailability.Update(availability);
            _context.SaveChanges();
        }

        public void DeleteSpotAvailability(SpotAvailability availability)
        {
            _context.SpotAvailability.Remove(availability);
            _context.SaveChanges();
        }

        public List<SpotAvailability> GetAvailabilityBySpotId(int spotId)
        {
            return _context.SpotAvailability
                .Where(a => a.SpotId == spotId)
                .OrderBy(a => a.StartTime)
                .ToList();
        }
    }
}