using BLL.Interfaces;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.Extensions;                    // בשביל User.GetUserId()

namespace Parking.Controllers
{
    // אין כאן בדיקות null: אם ההזמנה לא נמצאה, הסרוויס זורק שגיאה וה-GlobalExceptionFilter מחזיר 404
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult<List<PaymentDTO>> GetAllPayments()
        {
            return Ok(_paymentService.GetAllPayments());
        }

        [Authorize(Roles = "Driver,Admin")]     // רק נהג או מנהל
        [HttpGet("reservation/{reservationId}")]
        public ActionResult<List<PaymentDTO>> GetPaymentsByReservationId(int reservationId)
        {
            // מנהל רואה הכל, נהג רק את ההזמנות שלו
            if (User.IsInRole("Admin"))
            {
                return Ok(_paymentService.GetPaymentsByReservationId(reservationId));
            }

            return Ok(_paymentService.GetMyPaymentsByReservationId(reservationId, User.GetUserId()));
        }

        [Authorize(Roles = "Driver")]
        [HttpPost]
        public ActionResult<PaymentDTO> CreatePayment([FromBody] PaymentCreateDTO dto)
        {
            // בדיקת ModelState לא צריך: [ApiController] מחזיר 400 אוטומטית אם ה-DTO לא תקין
            // מזהה הנהג מגיע מהטוקן
            return Ok(_paymentService.CreatePayment(dto, User.GetUserId()));
        }
    }
}