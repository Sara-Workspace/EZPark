using DTO;                                                              // כדי להכיר את מחלקות ה-DTO

namespace BLL.Repositories                                              // הקובץ נמצא בתיקייה Repositories שב-BLL
{
    public interface ISpotAvailabilityRepository                        // ממשק: רשימת הפעולות על זמינות חניות
    {
        List<SpotAvailabilityDTO> GetAllSpotAvailabilities();           // מחזיר את כל רשומות הזמינות
        SpotAvailabilityDTO? GetSpotAvailabilityById(int id);           // מחזיר רשומה לפי קוד (null אם לא נמצאה)
        SpotAvailabilityDTO CreateSpotAvailability(SpotAvailabilityCreateDTO dto); // מוסיף זמינות חדשה ומחזיר אותה
        SpotAvailabilityDTO? UpdateSpotAvailability(int id, SpotAvailabilityDTO dto); // מעדכן זמינות (null אם לא נמצאה)
        bool DeleteSpotAvailability(int id);                            // מוחק זמינות (true אם הצליח)

        List<SpotAvailabilityDTO> GetAvailabilityBySpotId(int spotId);  // נוסף: הזמינות של חניה מסוימת
    }
}