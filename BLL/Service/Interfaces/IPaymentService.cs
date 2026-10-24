using DTO;

namespace BLL.Interfaces
{
    public interface IPaymentService
    {
        List<PaymentDTO> GetAllPayments();
        PaymentDTO CreatePayment(PaymentCreateDTO dto);
        List<PaymentDTO> GetPaymentsByReservationId(int reservationId);
    }
}