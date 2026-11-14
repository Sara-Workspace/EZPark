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

            return _context.Drivers
                .Include(d => d.User)
                .Include(d => d.Reservations)
                .ToList();
        }

        public Driver? GetDriverById(int id)
        {
            return _context.Drivers
                .Include(d => d.User)            // ← נוסף
                .FirstOrDefault(d => d.DriverID == id);
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
            return _context.Drivers
                .Include(d => d.User)            // ← נוסף
                .Include(d => d.Reservations)
                .FirstOrDefault(d => d.User.Email == email);
        }
        public async Task<Driver?> GetByEmailAsync(string email)
        {
            return await _context.Drivers
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.User.Email == email);
        }

        // בודק בטבלת Users המשותפת: המייל תפוס גם אם הוא שייך לנהג וגם אם לבעל חניה
        // Any = "יש לפחות שורה אחת כזאת?" - מחזיר true/false בלי לשלוף את כל הנתונים
        public bool EmailExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }
    }
}