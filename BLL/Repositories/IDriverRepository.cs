using System;
using System.Collections.Generic;//כדי להשתמש ב-list
using System.Linq;// פעולות שאילתה על רשימות (Where, Any וכו')
using System.Text;// עבודה עם טקסט (לא חובה, מגיע אוטומטית)
using System.Threading.Tasks;  // עבודה אסינכרונית (לא חובה, מגיע אוטומטית)
using DTO; // כדי להשתמש ב-DriverDTO מפרויקט ה-DTO


namespace BLL.Repositories//המקום איפה הקובץ יושב
{
    public interface IDriverRepository                       // ממשק ציבורי: רשימת הפעולות המותרות על נהגים
    {
        List<DriverDTO> GetAllDrivers();                     // מחזיר רשימה של כל הנהגים
        DriverDTO? GetDriverById(int id);                    // מחזיר נהג לפי קוד (? = אולי null אם לא נמצא)
        DriverDTO CreateDriver(DriverDTO dto);              // מוסיף נהג חדש ומחזיר אותו
        bool DeleteDriver(int id);                          // מוחק נהג, true אם הצליח
        DriverDTO? UpdateDriver(int id, DriverDTO dto);     // מעדכן נהג לפי קוד, null אם לא נמצא
        DriverDTO? GetDriverByEmail(string email);          // פונקציה ספציפית: מציאת נהג לפי מייל (להתחברות)
    }
}
