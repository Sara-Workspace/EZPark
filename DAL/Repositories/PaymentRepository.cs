using DAL;

namespace DAL.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ParkingDbContext _context;

        public PaymentRepository(ParkingDbContext context)
        {
            _context = context;
        }

        public List<Payment> GetAllPayments()
        {
            return _context.Payment.ToList();
        }

        public Payment? GetPaymentById(int id)
        {
            return _context.Payment.Find(id);
        }

        public void AddPayment(Payment payment)
        {
            _context.Payment.Add(payment);
            _context.SaveChanges();
        }

        public void UpdatePayment(Payment payment)
        {
            _context.Payment.Update(payment);
            _context.SaveChanges();
        }

        public void DeletePayment(Payment payment)
        {
            _context.Payment.Remove(payment);
            _context.SaveChanges();
        }

        public List<Payment> GetPaymentsByReservationId(int reservationId)
        {
            return _context.Payment
                .Where(p => p.ReservationID == reservationId)
                .ToList();
        }
    }
}