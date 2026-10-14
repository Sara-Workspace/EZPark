using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;                                      // PaymentDTO


namespace BLL.Repositories
{
    public interface IPaymentRepository
    {
        List<PaymentDTO> GetAllPayments();                      // כל התשלומים
        PaymentDTO? GetPaymentById(int id);                     // תשלום לפי קוד, או null
        PaymentDTO CreatePayment(PaymentDTO dto);               // תיעוד תשלום חדש
        PaymentDTO? UpdatePayment(int id, PaymentDTO dto);      // עדכון תשלום
        bool DeletePayment(int id);                             // מחיקת תשלום

        List<PaymentDTO> GetPaymentsByReservationId(int reservationId); // ספציפית: התשלומים של הזמנה מסוימת
    }
}
