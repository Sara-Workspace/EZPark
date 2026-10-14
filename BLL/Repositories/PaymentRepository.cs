using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using DTO;

namespace BLL.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ParkingDbContext _context;
        private readonly IMapper _mapper;

        public PaymentRepository(ParkingDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<PaymentDTO> GetAllPayments()
        {
            return _context.Payment                     // שימי לב: הטבלה נקראת Payment (ביחיד), כמו ב-DbContext
                .ProjectTo<PaymentDTO>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public PaymentDTO? GetPaymentById(int id)
        {
            return _context.Payment
                .Where(p => p.PaymentID == id)
                .ProjectTo<PaymentDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefault();
        }

        public PaymentDTO CreatePayment(PaymentCreateDTO dto)
        {
            var payment = _mapper.Map<Payment>(dto);
            payment.PaidAt = DateTime.Now;              // שעת התשלום = עכשיו
            payment.Status = "Paid";                    // סטטוס: שולם

            _context.Payment.Add(payment);
            _context.SaveChanges();

            return _mapper.Map<PaymentDTO>(payment);
        }

        public PaymentDTO? UpdatePayment(int id, PaymentDTO dto)
        {
            var payment = _context.Payment.Find(id);
            if (payment == null) return null;

            _mapper.Map(dto, payment);
            _context.SaveChanges();

            return _mapper.Map<PaymentDTO>(payment);
        }

        public bool DeletePayment(int id)
        {
            var payment = _context.Payment.Find(id);
            if (payment == null) return false;

            _context.Payment.Remove(payment);
            _context.SaveChanges();
            return true;
        }

        public List<PaymentDTO> GetPaymentsByReservationId(int reservationId)
        {
            return _context.Payment
                .Where(p => p.ReservationID == reservationId)
                .ProjectTo<PaymentDTO>(_mapper.ConfigurationProvider)
                .ToList();
        }
    }
}