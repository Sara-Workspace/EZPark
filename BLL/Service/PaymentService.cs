using AutoMapper;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL;
using DAL.Repositories;
using DTO;

namespace BLL.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IReservationsRepository _reservationRepo;
        private readonly IMapper _mapper;

        public PaymentService(
            IPaymentRepository paymentRepo,
            IReservationsRepository reservationRepo,
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

        // משלמים רק על הזמנה של הנהג המחובר
        public PaymentDTO CreatePayment(PaymentCreateDTO dto, int driverId)
        {
            // ההזמנה לא קיימת או לא שלו - אותה תשובה (404), כדי לא לגלות הזמנות של אחרים
            var reservation = _reservationRepo.GetReservationById(dto.ReservationID);
            if (reservation == null || reservation.DriverID != driverId)
                throw new NotFoundException($"הזמנה מספר {dto.ReservationID} לא נמצאה.");

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

        // לנהג: רק תשלומים של הזמנה שלו
        public List<PaymentDTO> GetMyPaymentsByReservationId(int reservationId, int driverId)
        {
            var reservation = _reservationRepo.GetReservationById(reservationId);
            if (reservation == null || reservation.DriverID != driverId)
                throw new NotFoundException($"הזמנה מספר {reservationId} לא נמצאה.");

            return GetPaymentsByReservationId(reservationId);
        }
    }
}