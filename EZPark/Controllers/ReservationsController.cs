using BLL.Services;
using BLL.Interfaces;
using DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Parking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationsService _reservationsService;

        public ReservationsController(IReservationsService reservationsService)
        {
            _reservationsService = reservationsService;
        }

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

        [HttpGet("driver/{driverId}")]
        public ActionResult<List<ReservationDTO>> GetReservationsByDriverId(int driverId)
        {
            var reservations = _reservationsService.GetReservationsByDriverId(driverId);
            return Ok(reservations);
        }

        [HttpPost]
        public ActionResult<ReservationDTO> CreateReservation([FromBody] ReservationCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdReservation = _reservationsService.CreateReservation(dto);
            return CreatedAtAction(nameof(GetReservationById), new { id = createdReservation.ReservationID }, createdReservation);
        }
    }
}
