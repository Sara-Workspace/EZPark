using DAL;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class DriverRepository : IDriverRepository
    {
        private readonly ParkingDbContext _context;

        public DriverRepository(ParkingDbContext context)
        {
            _context = context;
        }

        public List<Driver> GetAllDrivers()
        {
            return _context.Drivers.ToList();
        }

        public Driver? GetDriverById(int id)
        {
            return _context.Drivers.Find(id);
        }

        public void AddDriver(Driver driver)
        {
            _context.Drivers.Add(driver);
            _context.SaveChanges();
        }

        public void UpdateDriver(Driver driver)
        {
            _context.Drivers.Update(driver);
            _context.SaveChanges();
        }

        public void DeleteDriver(Driver driver)
        {
            _context.Drivers.Remove(driver);
            _context.SaveChanges();
        }

        public Driver? GetDriverByEmail(string email)
        {
            return _context.Drivers.FirstOrDefault(d => d.Email == email);
        }
    }
}