using DTO;                                                      // כדי להכיר את מחלקות ה-DTO

namespace BLL.Repositories                                      // הקובץ נמצא בתיקייה Repositories שב-BLL
{
    public interface IParkingSpotRepository                     // ממשק: רשימת הפעולות על חניות
    {
        List<ParkingSpotDTO> GetAllParkingSpots();              // מחזיר את כל החניות
        ParkingSpotDTO? GetParkingSpotById(int id);             // מחזיר חניה לפי קוד (null אם לא נמצאה)
        ParkingSpotDTO CreateParkingSpot(ParkingSpotCreateDTO dto); // מוסיף חניה חדשה ומחזיר אותה
        ParkingSpotDTO? UpdateParkingSpot(int id, ParkingSpotDTO dto); // מעדכן חניה (null אם לא נמצאה)
        bool DeleteParkingSpot(int id);                         // מוחק חניה (true אם הצליח)

        List<ParkingSpotDTO> GetAvailableParkingSpots();        // נוסף: רק חניות פעילות שיש להן זמן פנוי
        List<ParkingSpotDTO> GetParkingSpotsByOwnerId(int ownerId); // נוסף: החניות של בעלים מסוים
        List<ParkingSpotDTO> GetParkingSpotsByCity(string city);    // נוסף: חיפוש חניות לפי עיר
        ParkingSpotDetailedDTO? GetParkingSpotDetailed(int id);     // נוסף: חניה + פרטי הבעלים
        ParkingSpotWithReviewsDTO? GetParkingSpotWithReviews(int id); // נוסף: חניה + הביקורות עליה
    }
}