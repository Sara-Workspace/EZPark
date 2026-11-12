using BLL.Services;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Parking.Extensions;

namespace Parking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationsController : ControllerBase
    {
        private readonly ReservationsService _reservationsService;

        public ReservationsController(ReservationsService reservationsService)
        {
            _reservationsService = reservationsService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult<List<ReservationDTO>> GetAllReservations()
        {
            var reservations = _reservationsService.GetAllReservations();
            return Ok(reservations);
        }

        [HttpGet("{id}")]
        public ActionResult<ReservationDTO> GetReservationById(int id)
        {
            var reservation = _reservationsService.GetReservationById(id);
            if (reservation == null)
            {
                return NotFound($"Reservation with ID {id} not found.");
            }

            // תיקון 4: רק הנהג שהזמין, או מנהל
            bool isMine = User.IsInRole("Driver") && reservation.DriverID == User.GetUserId();
            if (!isMine && !User.IsInRole("Admin"))
            {
                return NotFound($"Reservation with ID {id} not found.");
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

            var reservations = _reservationsService.GetReservationsByDriverId(driverId);
            return Ok(reservations);
        }

        [Authorize(Roles = "Driver")]
        [HttpPost]
        public ActionResult<ReservationDTO> CreateReservation([FromBody] ReservationCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // ← תיקון 5: מזהה הנהג מהטוקן, מועבר כפרמטר
            var createdReservation = _reservationsService.CreateReservation(dto, User.GetUserId());
            return CreatedAtAction(nameof(GetReservationById), new { id = createdReservation.ReservationID }, createdReservation);
        }
    }
}