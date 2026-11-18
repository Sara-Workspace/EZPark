using DTO;

namespace BLL.Interfaces
{
    public interface IPaymentService
    {
        List<PaymentDTO> GetAllPayments();
        PaymentDTO CreatePayment(PaymentCreateDTO dto, int driverId);                       // זורק NotFoundException אם ההזמנה לא קיימת / לא שלו
        List<PaymentDTO> GetPaymentsByReservationId(int reservationId);
        List<PaymentDTO> GetMyPaymentsByReservationId(int reservationId, int driverId);     // זורק NotFoundException אם ההזמנה לא קיימת / לא שלו
    }
}