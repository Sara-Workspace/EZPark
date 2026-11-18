using DAL;
using DAL.Repositories;


namespace DAL.Repositories
{
    public interface IDriverRepository
    {
        List<Driver> GetAllDrivers();
        Driver? GetDriverById(int id);
        void AddDriver(Driver driver);
        void UpdateDriver(Driver driver);
        void DeleteDriver(Driver driver);
        Driver? GetDriverByEmail(string email);
        Task<Driver?> GetByEmailAsync(string email);
        bool EmailExists(string email);   // האם המייל כבר רשום במערכת (אצל נהג או בעל חניה)
        Driver? GetDriverByLicensePlate(string digitsOnly);   // מחפש נהג לפי מספר רישוי (רק ספרות, למשל "1234567")
    }
}