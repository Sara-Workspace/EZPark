using AutoMapper;
using DAL;
using DAL.Repositories;
using DTO;

namespace BLL.Services
{
    public class PaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IMapper _mapper;

        public PaymentService(IPaymentRepository paymentRepo, IMapper mapper)
        {
            _paymentRepo = paymentRepo;
            _mapper = mapper;
        }

        public List<PaymentDTO> GetAllPayments()
        {
            var payments = _paymentRepo.GetAllPayments();
            return _mapper.Map<List<PaymentDTO>>(payments);
        }

        // לוגיקה עסקית: אתחול שעת תשלום נוכחית וסטטוס "Paid"
        public PaymentDTO CreatePayment(PaymentCreateDTO dto)
        {
            var payment = _mapper.Map<Payment>(dto);
            payment.PaidAt = DateTime.Now;
            payment.Status = "Paid";

            _paymentRepo.AddPayment(payment);
            return _mapper.Map<PaymentDTO>(payment);
        }

        public List<PaymentDTO> GetPaymentsByReservationId(int reservationId)
        {
            var payments = _paymentRepo.GetPaymentsByReservationId(reservationId);
            return _mapper.Map<List<PaymentDTO>>(payments);
        }
    }
}