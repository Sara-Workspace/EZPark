using BLL.Exceptions;
using BLL.Interfaces;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.Extensions;

namespace Parking.Controllers
{
    // אין כאן בדיקות null: אם ההזמנה לא נמצאה, הסרוויס זורק שגיאה וה-GlobalExceptionFilter מחזיר 404
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationsService _reservationsService;

        public ReservationsController(IReservationsService reservationsService)
        {
            _reservationsService = reservationsService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult<List<ReservationDTO>> GetAllReservations()
        {
            return Ok(_reservationsService.GetAllReservations());
        }

        [HttpGet("{id}")]
        public ActionResult<ReservationDTO> GetReservationById(int id)
        {
            var reservation = _reservationsService.GetReservationById(id);

            // רק הנהג שהזמין, או מנהל.
            // הזמנה של מישהו אחר - עונים "לא נמצאה" (404), כדי לא לגלות שהיא קיימת
            bool isMine = User.IsInRole("Driver") && reservation.DriverID == User.GetUserId();
            if (!isMine && !User.IsInRole("Admin"))
            {
                throw new NotFoundException($"הזמנה מספר {id} לא נמצאה.");
            }

            return Ok(reservation);
        }

        [Authorize(Roles = "Driver")]
        [HttpGet("driver/{driverId}")]
        public ActionResult<List<ReservationDTO>> GetReservationsByDriverId(int driverId)
        {
            // וידוא שהנהג מורשה לצפות אך ורק בהזמנות של עצמו
            if (driverId != User.GetUserId())
            {
                return Forbid();
            }

            return Ok(_reservationsService.GetReservationsByDriverId(driverId));
        }

        [Authorize(Roles = "Driver")]
        [HttpPost]
        public ActionResult<ReservationDTO> CreateReservation([FromBody] ReservationCreateDTO dto)
        {
            // בדיקת ModelState לא צריך: [ApiController] מחזיר 400 אוטומטית אם ה-DTO לא תקין
            // מזהה הנהג מגיע מהטוקן, ומועבר כפרמטר
            var createdReservation = _reservationsService.CreateReservation(dto, User.GetUserId());
            return CreatedAtAction(nameof(GetReservationById), new { id = createdReservation.ReservationID }, createdReservation);
        }
    }
}