using DTO;

namespace BLL.Interfaces
{
    public interface IDriverService
    {
        Task<DriverDTO?> ValidateDriverCredentialsAsync(string email, string password);   // התחברות
        List<DriverDTO> GetAllDrivers();
        DriverDTO? GetDriverById(int id);
        DriverDTO CreateDriver(DriverCreateDTO dto);
        DriverDTO? UpdateDriver(int id, DriverUpdateDTO dto);
        bool DeleteDriver(int id);
        DriverDTO? TopUpWallet(int driverId, decimal amount);
    }
}
