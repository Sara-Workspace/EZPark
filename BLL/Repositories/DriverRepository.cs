using AutoMapper;                           // ספריית ההמרות
using AutoMapper.QueryableExtensions;       // נותן את ProjectTo (המרה בתוך השאילתה)
using DAL;                                  // הטבלאות וה-DbContext
using DTO;                                  // מחלקות ה-DTO

namespace BLL.Repositories                  // הקובץ בתיקייה Repositories שב-BLL
{
    public class DriverRepository : IDriverRepository   // מממשת את הממשק IDriverRepository
    {
        private readonly ParkingDbContext _context;     // החיבור למסד הנתונים
        private readonly IMapper _mapper;               // הכלי שממיר בין טבלה ל-DTO

        public DriverRepository(ParkingDbContext context, IMapper mapper) // בנאי: ה-DI שולח לכאן את שני הכלים
        {
            _context = context;                         // שומר את החיבור למסד
            _mapper = mapper;                           // שומר את הממיר
        }

        public List<DriverDTO> GetAllDrivers()          // מחזיר את כל הנהגים
        {
            return _context.Drivers                     // ניגש לטבלת הנהגים
                .ProjectTo<DriverDTO>(_mapper.ConfigurationProvider) // ממיר כל שורה ל-DriverDTO
                .ToList();                              // מריץ את השאילתה ומחזיר רשימה
        }

        public DriverDTO? GetDriverById(int id)         // מחזיר נהג לפי קוד
        {
            return _context.Drivers                     // ניגש לטבלת הנהגים
                .Where(d => d.DriverID == id)           // מסנן: רק הנהג עם הקוד הזה
                .ProjectTo<DriverDTO>(_mapper.ConfigurationProvider) // ממיר ל-DriverDTO
                .FirstOrDefault();                      // מחזיר את הראשון, או null אם אין
        }

        public DriverDTO CreateDriver(DriverCreateDTO dto)  // מוסיף נהג חדש
        {
            var driver = _mapper.Map<Driver>(dto);      // ממיר את ה-DTO לשורה בטבלה
            driver.CreatedAt = DateTime.Now;            // קובע תאריך יצירה = עכשיו
            driver.CreditBalance = 0;                   // יתרה התחלתית = 0

            _context.Drivers.Add(driver);               // מוסיף לטבלה
            _context.SaveChanges();                     // שומר במסד (כאן נוצר ה-ID)

            return _mapper.Map<DriverDTO>(driver);      // מחזיר את הנהג החדש כ-DTO
        }

        public DriverDTO? UpdateDriver(int id, DriverUpdateDTO dto) // מעדכן נהג קיים
        {
            var driver = _context.Drivers.Find(id);     // מחפש את הנהג לפי קוד
            if (driver == null) return null;            // לא נמצא: מחזיר null

            _mapper.Map(dto, driver);                   // מעתיק את הערכים החדשים לנהג הקיים
            _context.SaveChanges();                     // שומר במסד

            return _mapper.Map<DriverDTO>(driver);      // מחזיר את הנהג המעודכן
        }

        public bool DeleteDriver(int id)                // מוחק נהג
        {
            var driver = _context.Drivers.Find(id);     // מחפש את הנהג לפי קוד
            if (driver == null) return false;           // לא נמצא: מחזיר false

            _context.Drivers.Remove(driver);            // מסמן למחיקה
            _context.SaveChanges();                     // מוחק מהמסד
            return true;                                // הצליח
        }

        public DriverDTO? GetDriverByEmail(string email) // מחפש נהג לפי מייל
        {
            return _context.Drivers                     // ניגש לטבלת הנהגים
                .Where(d => d.Email == email)           // מסנן: רק עם המייל הזה
                .ProjectTo<DriverDTO>(_mapper.ConfigurationProvider) // ממיר ל-DriverDTO
                .FirstOrDefault();                      // מחזיר את הראשון, או null
        }
    }
}