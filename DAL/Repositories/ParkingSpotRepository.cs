using DAL;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class ParkingSpotRepository : IParkingSpotRepository
    {
        private readonly ParkingDbContext _context;

        public ParkingSpotRepository(ParkingDbContext context)
        {
            _context = context;
        }
        public ParkingSpots? GetOwnedSpot(int spotId, int ownerId)
        {
            return _context.ParkingSpots
                .FirstOrDefault(s => s.SpotId == spotId && s.OwnerId == ownerId);
        }
        public List<ParkingSpots> GetAllParkingSpots()
        {
            return _context.ParkingSpots
                 .Include(p => p.Owner)
                 .ToList();
        }

        public ParkingSpots? GetParkingSpotById(int id)
        {
            return _context.ParkingSpots
                 .Include(p => p.Owner)
                 .FirstOrDefault(p => p.SpotId == id);
        }

        public void AddParkingSpot(ParkingSpots spot)
        {
            _context.ParkingSpots.Add(spot);
            _context.SaveChanges();
        }

        public void UpdateParkingSpot(ParkingSpots spot)
        {
            _context.ParkingSpots.Update(spot);
            _context.SaveChanges();
        }

        public void DeleteParkingSpot(ParkingSpots spot)
        {
            _context.ParkingSpots.Remove(spot);
            _context.SaveChanges();
        }

        public List<ParkingSpots> GetParkingSpotsByOwnerId(int ownerId)
        {
            return _context.ParkingSpots
                 .Include(p => p.Owner)
                 .Where(p => p.OwnerId == ownerId)
                 .ToList();
        }

        public List<ParkingSpots> GetParkingSpotsByCity(string city)
        {
            return _context.ParkingSpots
                .Include(p => p.Owner)
                .Where(p => p.City == city)
                .ToList();
        }
    }
}