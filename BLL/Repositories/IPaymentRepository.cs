using DTO;                                                      // כדי להכיר את מחלקות ה-DTO

namespace BLL.Repositories                                      // הקובץ נמצא בתיקייה Repositories שב-BLL
{
    public interface IPaymentRepository                         // ממשק: רשימת הפעולות על תשלומים
    {
        List<PaymentDTO> GetAllPayments();                      // מחזיר את כל התשלומים
        PaymentDTO? GetPaymentById(int id);                     // מחזיר תשלום לפי קוד (null אם לא נמצא)
        PaymentDTO CreatePayment(PaymentCreateDTO dto);         // מוסיף תשלום חדש ומחזיר אותו
        PaymentDTO? UpdatePayment(int id, PaymentDTO dto);      // מעדכן תשלום (null אם לא נמצא)
        bool DeletePayment(int id);                             // מוחק תשלום (true אם הצליח)

        List<PaymentDTO> GetPaymentsByReservationId(int reservationId); // נוסף: התשלומים של הזמנה מסוימת
    }
}