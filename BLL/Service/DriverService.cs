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
        private readonly IOwnersRepository _ownerRepo;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Driver> _passwordHasher;

        public DriverService(
            IDriverRepository driverRepo,
            IOwnersRepository ownerRepo,
            IMapper mapper,
            IPasswordHasher<Driver> passwordHasher)
        {
            _driverRepo = driverRepo;
            _ownerRepo = ownerRepo;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }
        // יוצר פרופיל נהג אם המשתמש נרשם במקור רק כבעל חניה, ומעדכן מספר רישוי נוכחי
        public DriverDTO EnsureDriverAndSetPlate(string email, string licensePlate)
        {
            if (string.IsNullOrWhiteSpace(licensePlate))
                throw new BusinessRuleException("חובה להזין מספר רישוי תקין.");

            var driver = _driverRepo.GetDriverByEmail(email);

            if (driver == null)
            {
                // המשתמש קיים במערכת כבעל חניה בלבד - נשלוף אותו וניצור לו פרופיל נהג מקושר לאותו UserId
                var owner = _ownerRepo.GetOwnerByEmail(email)
                    ?? throw new NotFoundException("המשתמש לא נמצא במערכת.");

                driver = new Driver
                {
                    UserId = owner.UserId,
                    CreditBalance = 0,
                    LicensePlate = licensePlate.Trim()
                };

                _driverRepo.AddDriver(driver);
            }
            else
            {
                driver.LicensePlate = licensePlate.Trim();
                _driverRepo.UpdateDriver(driver);
            }

            return _mapper.Map<DriverDTO>(driver);
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

            // כלל עסקי: אותו מייל לא יכול להירשם פעמיים - השומר מחזיר 409
            if (_driverRepo.EmailExists(dto.Email))
                throw new ConflictException("כתובת המייל כבר רשומה במערכת.");

            var driver = new Driver
            {
                CreditBalance = 0,
                //PaymentMethodID = dto.PaymentMethodID ?? 0,
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

        // פעולה חישובית/עסקית: טעינת ארנק דיגיטלי לנהג
        public DriverDTO TopUpWallet(int driverId, decimal amount)
        {
            // כלל עסקי: אי אפשר לטעון סכום אפס או שלילי
            if (amount <= 0)
                throw new BusinessRuleException("סכום הטעינה חייב להיות חיובי.");

            // בדיקה שהנהג קיים
            var driver = _driverRepo.GetDriverById(driverId)
                ?? throw new NotFoundException($"נהג מספר {driverId} לא נמצא.");

            // חישוב עדכון היתרה
            driver.CreditBalance += amount;
            _driverRepo.UpdateDriver(driver);

            return _mapper.Map<DriverDTO>(driver);
        }

        // עדכון מספר הרישוי של הרכב הנוכחי (דורס את הקודם)
        public DriverDTO? UpdateLicensePlate(int driverId, string licensePlate)
        {
            var driver = _driverRepo.GetDriverById(driverId);
            if (driver == null) return null;

            driver.LicensePlate = licensePlate.Trim();
            _driverRepo.UpdateDriver(driver);

            return _mapper.Map<DriverDTO>(driver);
        }
    }
}