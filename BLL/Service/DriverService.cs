// שכבת המטבח - בה כתובים כל החוקים והלוגיקה העסקית

using AutoMapper;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL;
using DAL.Repositories;
using DTO;
using Microsoft.AspNetCore.Identity;

namespace BLL
{
    public class DriverService : IDriverService
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
        // כאן null זה לא שגיאה, אלא תשובה "הפרטים לא נכונים".
        // ה-AuthController ממשיך לבדוק בעל חניה, ורק אם שניהם null מחזיר 401
        public async Task<DriverDTO?> ValidateDriverCredentialsAsync(string email, string password)
        {
            // 1. שליפת הנהג לפי דוא"ל מתוך ה-Repository
            var driver = await _driverRepo.GetByEmailAsync(email);
            if (driver == null || driver.User == null)
                return null;

            // 2. אימות הסיסמה המוכנסת מול ה-Hash השמור במסד הנתונים
            var verificationResult = _passwordHasher.VerifyHashedPassword(driver, driver.User.PasswordHash, password);
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

        public DriverDTO GetDriverById(int id)
        {
            // אם הנהג לא קיים - זורקים שגיאה, והשומר (GlobalExceptionFilter) מחזיר 404
            var driver = _driverRepo.GetDriverById(id)
                ?? throw new NotFoundException($"נהג מספר {id} לא נמצא.");

            return _mapper.Map<DriverDTO>(driver);
        }

        // לוגיקה עסקית / חישובית: קביעת תאריך יצירה, איפוס יתרה התחלתית ובדיקות תקינות
        public DriverDTO CreateDriver(DriverCreateDTO dto)
        {
            // ArgumentException - השומר מחזיר 400
            if (string.IsNullOrWhiteSpace(dto.PasswordHash))
            {
                throw new ArgumentException("חובה להזין סיסמה בעת יצירת נהג חדש");
            }

            var driver = new Driver
            {
                CreditBalance = 0,
                PaymentMethodID = dto.PaymentMethodID ?? 0,
                User = new Users
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    IsAdmin = false,
                    CreatedAt = DateTime.UtcNow
                }
            };

            // הצפנת הסיסמה באמצעות ה-IPasswordHasher שכבר מוזרק למחלקה
            driver.User.PasswordHash = _passwordHasher.HashPassword(driver, dto.PasswordHash);

            _driverRepo.AddDriver(driver);

            return _mapper.Map<DriverDTO>(driver);
        }

        public DriverDTO UpdateDriver(int id, DriverUpdateDTO dto)
        {
            var driver = _driverRepo.GetDriverById(id)
                ?? throw new NotFoundException($"נהג מספר {id} לא נמצא.");

            if (driver.User != null)
            {
                driver.User.FirstName = dto.FirstName;
                driver.User.LastName = dto.LastName;
                driver.User.Phone = dto.Phone;
            }

            _driverRepo.UpdateDriver(driver);

            return _mapper.Map<DriverDTO>(driver);
        }

        public void DeleteDriver(int id)
        {
            var driver = _driverRepo.GetDriverById(id)
                ?? throw new NotFoundException($"נהג מספר {id} לא נמצא.");

            _driverRepo.DeleteDriver(driver);
        }

        // פעולה חישובית/עסקית: טעינת ארנק דיגיטלי לנהג
        public DriverDTO TopUpWallet(int driverId, decimal amount)
        {
            // כלל עסקי: אי אפשר לטעון סכום אפס או שלילי - השומר מחזיר 400
            if (amount <= 0)
                throw new BusinessRuleException("סכום הטעינה חייב להיות חיובי.");

            var driver = _driverRepo.GetDriverById(driverId)
                ?? throw new NotFoundException($"נהג מספר {driverId} לא נמצא.");

            // חישוב עדכון היתרה
            driver.CreditBalance += amount;
            _driverRepo.UpdateDriver(driver);

            return _mapper.Map<DriverDTO>(driver);
        }
    }
}