using BLL.Services;
using BLL.Interfaces;
using DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Parking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public ActionResult<List<PaymentDTO>> GetAllPayments()
        {
            var payments = _paymentService.GetAllPayments();
            return Ok(payments);
        }

        [HttpGet("reservation/{reservationId}")]
        public ActionResult<List<PaymentDTO>> GetPaymentsByReservationId(int reservationId)
        {
            var payments = _paymentService.GetPaymentsByReservationId(reservationId);
            return Ok(payments);
        }

        [HttpPost]
        public ActionResult<PaymentDTO> CreatePayment([FromBody] PaymentCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdPayment = _paymentService.CreatePayment(dto);
            return Ok(createdPayment);
        }
    }
}
