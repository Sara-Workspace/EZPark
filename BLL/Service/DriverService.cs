// שכבת המטבח - בה כתובים כל החוקים והלוגיקה העסקית

using AutoMapper;
using DAL;
using DAL.Repositories;
using DTO;
using Microsoft.AspNetCore.Identity;

namespace BLL
{
    public class DriverService
    {
        private readonly IDriverRepository _driverRepo;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Driver> _passwordHasher;

        public DriverService(
            IDriverRepository driverRepo,
            IMapper mapper,
            IPasswordHasher<Driver> passwordHasher)
        {
            _driverRepo = driverRepo;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        // --- מתודת האימות להתחברות (Login) ---
        public async Task<DriverDTO?> ValidateDriverCredentialsAsync(string email, string password)
        {
            // 1. שליפת הנהג לפי דוא"ל מתוך ה-Repository
            var driver = await _driverRepo.GetByEmailAsync(email);
            if (driver == null)
                return null;

            // 2. אימות הסיסמה המוכנסת מול ה-Hash השמור במסד הנתונים
            var verificationResult = _passwordHasher.VerifyHashedPassword(driver, driver.PasswordHash, password);
            if (verificationResult == PasswordVerificationResult.Failed)
                return null;

            // 3. החזרת ה-DTO של הנהג במידה וההתחברות הצליחה
            return _mapper.Map<DriverDTO>(driver);
        }

        public List<DriverDTO> GetAllDrivers()
        {
            var drivers = _driverRepo.GetAllDrivers();
            return _mapper.Map<List<DriverDTO>>(drivers);
        }

        public DriverDTO? GetDriverById(int id)
        {
            var driver = _driverRepo.GetDriverById(id);
            if (driver == null) return null;
            return _mapper.Map<DriverDTO>(driver);
        }

        // לוגיקה עסקית / חישובית: קביעת תאריך יצירה, איפוס יתרה התחלתית ובדיקות תקינות
        public DriverDTO CreateDriver(DriverCreateDTO dto)
        {
            var driver = _mapper.Map<Driver>(dto);

            // לוגיקה עסקית: אתחול ערכים ראשוניים למשתמש חדש
            driver.CreatedAt = DateTime.Now;
            driver.CreditBalance = 0;

            // הצפנת הסיסמה ברישום נהג חדש
            if (!string.IsNullOrEmpty(dto.PasswordHash))
            {
                driver.PasswordHash = _passwordHasher.HashPassword(driver, dto.PasswordHash);
            }

            _driverRepo.AddDriver(driver);

            return _mapper.Map<DriverDTO>(driver);
        }

        public DriverDTO? UpdateDriver(int id, DriverUpdateDTO dto)
        {
            var driver = _driverRepo.GetDriverById(id);
            if (driver == null) return null;

            _mapper.Map(dto, driver);
            _driverRepo.UpdateDriver(driver);

            return _mapper.Map<DriverDTO>(driver);
        }

        public bool DeleteDriver(int id)
        {
            var driver = _driverRepo.GetDriverById(id);
            if (driver == null) return false;

            _driverRepo.DeleteDriver(driver);
            return true;
        }

        // פעולה חישובית/עסקית לדוגמה: טעינת ארנק דיגיטלי לנהג
        public DriverDTO? TopUpWallet(int driverId, decimal amount)
        {
            var driver = _driverRepo.GetDriverById(driverId);
            if (driver == null) return null;

            if (amount <= 0) throw new ArgumentException("סכום הטעינה חייב להיות חיובי");

            // חישוב עדכון היתרה
            driver.CreditBalance += amount;
            _driverRepo.UpdateDriver(driver);

            return _mapper.Map<DriverDTO>(driver);
        }
    }
}