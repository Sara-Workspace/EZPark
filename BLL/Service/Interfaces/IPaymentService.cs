using DTO;

namespace BLL.Interfaces
{
    public interface IPaymentService
    {
        List<PaymentDTO> GetAllPayments();
        PaymentDTO? CreatePayment(PaymentCreateDTO dto, int driverId);
        List<PaymentDTO> GetPaymentsByReservationId(int reservationId);
        List<PaymentDTO>? GetMyPaymentsByReservationId(int reservationId, int driverId);
    }
}
