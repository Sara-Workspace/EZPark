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
                .OrderBy(a => a.DayOfWeek)      // קודם לפי יום (ראשון, שני...)
                .ThenBy(a => a.StartTime)       // ובתוך אותו יום לפי שעה
                .ToList();
        }

        // שעות הזמינות של כמה חניות בשאילתה אחת (ולא שאילתה נפרדת לכל חניה).
        // בלי Include: בחיפוש צריך רק את היום והשעות, לא את פרטי החניה
        public List<SpotAvailability> GetAvailabilityForSpots(List<int> spotIds)
        {
            return _context.SpotAvailability
                .Where(a => spotIds.Contains(a.SpotId))
                .ToList();
        }
    }
}