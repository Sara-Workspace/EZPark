using AutoMapper;
using DAL;
using DAL.Repositories;
using DTO;

namespace BLL.Services
{
    public class PaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IReservationsRepository _reservationRepo;   // ← תיקון 4
        private readonly IMapper _mapper;

        public PaymentService(
            IPaymentRepository paymentRepo,
            IReservationsRepository reservationRepo,                 // ← תיקון 4
            IMapper mapper)
        {
            _paymentRepo = paymentRepo;
            _reservationRepo = reservationRepo;
            _mapper = mapper;
        }

        public List<PaymentDTO> GetAllPayments()
        {
            var payments = _paymentRepo.GetAllPayments();
            return _mapper.Map<List<PaymentDTO>>(payments);
        }

        // ← תיקון 4: משלמים רק על הזמנה של הנהג המחובר
        public PaymentDTO? CreatePayment(PaymentCreateDTO dto, int driverId)
        {
            var reservation = _reservationRepo.GetReservationById(dto.ReservationID);
            if (reservation == null || reservation.DriverID != driverId)
                return null;   // ההזמנה לא קיימת, או לא שלו

            var payment = _mapper.Map<Payment>(dto);
            payment.PaidAt = DateTime.Now;
            payment.Status = "Paid";

            _paymentRepo.AddPayment(payment);
            return _mapper.Map<PaymentDTO>(payment);
        }

        // למנהל: תשלומים של כל הזמנה
        public List<PaymentDTO> GetPaymentsByReservationId(int reservationId)
        {
            var payments = _paymentRepo.GetPaymentsByReservationId(reservationId);
            return _mapper.Map<List<PaymentDTO>>(payments);
        }

        // ← תיקון 4: לנהג, רק תשלומים של הזמנה שלו
        public List<PaymentDTO>? GetMyPaymentsByReservationId(int reservationId, int driverId)
        {
            var reservation = _reservationRepo.GetReservationById(reservationId);
            if (reservation == null || reservation.DriverID != driverId)
                return null;   // ההזמנה לא קיימת, או לא שלו

            return GetPaymentsByReservationId(reservationId);
        }
    }
}