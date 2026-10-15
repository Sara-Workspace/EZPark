using DAL;

namespace DAL.Repositories
{
    public interface IPaymentRepository
    {
        List<Payment> GetAllPayments();
        Payment? GetPaymentById(int id);
        void AddPayment(Payment payment);
        void UpdatePayment(Payment payment);
        void DeletePayment(Payment payment);
        List<Payment> GetPaymentsByReservationId(int reservationId);
    }
}