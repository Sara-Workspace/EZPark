using DAL;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class OwnersRepository : IOwnersRepository
    {
        private readonly ParkingDbContext _context;

        public OwnersRepository(ParkingDbContext context)
        {
            _context = context;
        }

        public List<Owners> GetAllOwners()
        {
            return _context.Owners
                .Include(o=>o.ParkingSpots)
                .ToList();
        }

        public Owners? GetOwnerById(int id)
        {
            return _context.Owners
                .Include(o => o.ParkingSpots)
                .FirstOrDefault(o => o.OwnerID == id);
        }

        public void AddOwner(Owners owner)
        {
            _context.Owners.Add(owner);
            _context.SaveChanges();
        }

        public void UpdateOwner(Owners owner)
        {
            _context.Owners.Update(owner);
            _context.SaveChanges();
        }

        public void DeleteOwner(Owners owner)
        {
            _context.Owners.Remove(owner);
            _context.SaveChanges();
        }

        public Owners? GetOwnerByEmail(string email)
        {
            return _context.Owners
                .Include(o => o.ParkingSpots)
                .FirstOrDefault(o => o.Email == email);
        }

        public Owners? GetOwnerWithSpots(int id)
        {
            return _context.Owners
                .Include(o => o.ParkingSpots)
                .FirstOrDefault(o => o.OwnerID == id);
        }
    }
}