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

            // חילוץ מזהה הנהג מתוך ה-JWT Token והשמתו ב-DTO למניעת זיוף מזהה
            dto.DriverId = User.GetUserId(); // (אם ב-DTO השדה נקרא DriverId ב-i קטנה, עדכני בהתאם)

            var createdReservation = _reservationsService.CreateReservation(dto);
            return CreatedAtAction(nameof(GetReservationById), new { id = createdReservation.ReservationID }, createdReservation);
        }
    }
}