using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;                                      // ReservationDTO


namespace BLL.Repositories
{
    public interface IReservationsRepository
    {
        List<ReservationDTO> GetAllReservations();                          // כל ההזמנות
        ReservationDTO? GetReservationById(int id);                         // הזמנה לפי קוד, או null
        ReservationDTO CreateReservation(ReservationDTO dto);               // יצירת הזמנה חדשה (השכרת חניה)
        ReservationDTO? UpdateReservation(int id, ReservationDTO dto);      // עדכון הזמנה
        bool DeleteReservation(int id);                                     // מחיקת/ביטול הזמנה

        List<ReservationDTO> GetReservationsByDriverId(int driverId);       // ספציפית: ההזמנות של נהג מסוים
        List<ReservationDTO> GetReservationsBySpotId(int spotId);           // ספציפית: ההזמנות של חניה מסוימת
        List<ReservationDTO> GetReservationsByDateRange(DateTime from, DateTime to); // ספציפית: הזמנות בטווח תאריכים
    }
}
