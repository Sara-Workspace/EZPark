//זה שכבת המטבח- בה כתוב כל החוקים נגיד על החניון

using AutoMapper;
using DAL.Repositories;
using DTO;
using DAL;

namespace BLL
{
    public class DriverService
    {
        private readonly IDriverRepository _driverRepo;
        private readonly IMapper _mapper;

        public DriverService(IDriverRepository driverRepo, IMapper mapper)
        {
            _driverRepo = driverRepo;
            _mapper = mapper;
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