using BLL.Services;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.Extensions;                    // ← תיקון 4: בשביל User.GetUserId()

namespace Parking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult<List<PaymentDTO>> GetAllPayments()
        {
            var payments = _paymentService.GetAllPayments();
            return Ok(payments);
        }

        [Authorize(Roles = "Driver,Admin")]     // ← תיקון 4: רק נהג או מנהל
        [HttpGet("reservation/{reservationId}")]
        public ActionResult<List<PaymentDTO>> GetPaymentsByReservationId(int reservationId)
        {
            // ← תיקון 4: מנהל רואה הכל, נהג רק את ההזמנות שלו
            if (User.IsInRole("Admin"))
            {
                return Ok(_paymentService.GetPaymentsByReservationId(reservationId));
            }

            var payments = _paymentService.GetMyPaymentsByReservationId(reservationId, User.GetUserId());
            if (payments == null)
            {
                return NotFound($"Reservation with ID {reservationId} not found.");
            }
            return Ok(payments);
        }

        [Authorize(Roles = "Driver")]
        [HttpPost]
        public ActionResult<PaymentDTO> CreatePayment([FromBody] PaymentCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // ← תיקון 4: מזהה הנהג מהטוקן
            var createdPayment = _paymentService.CreatePayment(dto, User.GetUserId());
            if (createdPayment == null)
            {
                return NotFound($"Reservation with ID {dto.ReservationID} not found.");
            }
            return Ok(createdPayment);
        }
    }
}