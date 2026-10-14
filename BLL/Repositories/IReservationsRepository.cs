using DTO;                                                          // כדי להכיר את מחלקות ה-DTO

namespace BLL.Repositories                                          // הקובץ נמצא בתיקייה Repositories שב-BLL
{
    public interface IReservationsRepository                        // ממשק: רשימת הפעולות על הזמנות
    {
        List<ReservationDTO> GetAllReservations();                  // מחזיר את כל ההזמנות
        ReservationDTO? GetReservationById(int id);                 // מחזיר הזמנה לפי קוד (null אם לא נמצאה)
        ReservationDTO CreateReservation(ReservationCreateDTO dto); // יוצר הזמנה חדשה ומחזיר אותה
        ReservationDTO? UpdateReservation(int id, ReservationDTO dto); // מעדכן הזמנה (null אם לא נמצאה)
        bool DeleteReservation(int id);                             // מוחק הזמנה (true אם הצליח)

        List<ReservationDTO> GetReservationsByDriverId(int driverId);   // נוסף: ההזמנות של נהג מסוים
        List<ReservationDTO> GetReservationsBySpotId(int spotId);       // נוסף: ההזמנות של חניה מסוימת
        List<ReservationDTO> GetReservationsByDateRange(DateTime from, DateTime to); // נוסף: הזמנות בטווח תאריכים
    }
}