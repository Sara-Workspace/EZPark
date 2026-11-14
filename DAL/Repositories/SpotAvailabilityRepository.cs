using DAL;
using Microsoft.EntityFrameworkCore;

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
            return _context.SpotAvailability
                            .Include(a => a.ParkingSpot)
                            .ToList();
        }

        public SpotAvailability? GetSpotAvailabilityById(int id)
        {
            return _context.SpotAvailability
                            .Include(a => a.ParkingSpot)
                            .FirstOrDefault(a => a.AvailabilityId == id);
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
                .Include(a => a.ParkingSpot)
                .Where(a => a.SpotId == spotId)
                .OrderBy(a => a.StartTime)
                .ToList();
        }
    }
}