using DTO;                                              // כדי להכיר את מחלקות ה-DTO

namespace BLL.Repositories                              // הקובץ נמצא בתיקייה Repositories שב-BLL
{
    public interface IDriverRepository                  // ממשק: רשימת הפעולות על נהגים (בלי מימוש)
    {
        List<DriverDTO> GetAllDrivers();                // מחזיר את כל הנהגים
        DriverDTO? GetDriverById(int id);               // מחזיר נהג לפי קוד (null אם לא נמצא)
        DriverDTO CreateDriver(DriverCreateDTO dto);    // מוסיף נהג חדש ומחזיר אותו
        DriverDTO? UpdateDriver(int id, DriverUpdateDTO dto); // מעדכן נהג (null אם לא נמצא)
        bool DeleteDriver(int id);                      // מוחק נהג (true אם הצליח)

        DriverDTO? GetDriverByEmail(string email);      // נוסף: מחפש נהג לפי מייל (להתחברות)
    }
}