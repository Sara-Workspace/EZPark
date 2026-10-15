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
    }
}