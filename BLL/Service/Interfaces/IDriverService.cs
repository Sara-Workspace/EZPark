using DTO;

namespace BLL.Interfaces
{
    public interface IDriverService
    {
        Task<DriverDTO?> ValidateDriverCredentialsAsync(string email, string password);   // התחברות: null = פרטים שגויים
        List<DriverDTO> GetAllDrivers();
        DriverDTO GetDriverById(int id);                                                   // זורק NotFoundException אם לא נמצא
        DriverDTO CreateDriver(DriverCreateDTO dto);
        DriverDTO UpdateDriver(int id, DriverUpdateDTO dto);                               // זורק NotFoundException אם לא נמצא
        void DeleteDriver(int id);                                                         // זורק NotFoundException אם לא נמצא
        DriverDTO TopUpWallet(int driverId, decimal amount);                               // זורק BusinessRuleException אם הסכום לא חיובי
    }
}