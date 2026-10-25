using BLL.Services;
using DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Parking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingSpotsController : ControllerBase
    {
        private readonly ParkingSpotService _parkingSpotService;

        public ParkingSpotsController(ParkingSpotService parkingSpotService)
        {
            _parkingSpotService = parkingSpotService;
        }

        [HttpGet]
        public ActionResult<List<ParkingSpotDTO>> GetAllParkingSpots()
        {
            var spots = _parkingSpotService.GetAllParkingSpots();
            return Ok(spots);
        }

        [HttpGet("{id}")]
        public ActionResult<ParkingSpotDTO> GetParkingSpotById(int id)
        {
            var spot = _parkingSpotService.GetParkingSpotById(id);
            if (spot == null)
            {
                return NotFound($"Parking spot with ID {id} not found.");
            }
            return Ok(spot);
        }

        [HttpGet("owner/{ownerId}")]
        public ActionResult<List<ParkingSpotDTO>> GetParkingSpotsByOwnerId(int ownerId)
        {
            var spots = _parkingSpotService.GetParkingSpotsByOwnerId(ownerId);
            return Ok(spots);
        }

        [HttpPost]
        public ActionResult<ParkingSpotDTO> CreateParkingSpot([FromForm] ParkingSpotCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdSpot = _parkingSpotService.CreateParkingSpot(dto);
                return CreatedAtAction(nameof(GetParkingSpotById), new { id = createdSpot.SpotId }, createdSpot);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // הקובץ לא תקין (סוג או גודל)
            }
        }

        [HttpPut("{id}")]
        public ActionResult<ParkingSpotDTO> UpdateParkingSpot(int id, [FromBody] ParkingSpotDTO dto)
        {
            var updatedSpot = _parkingSpotService.UpdateParkingSpot(id, dto);
            if (updatedSpot == null)
            {
                return NotFound($"Parking spot with ID {id} not found.");
            }
            return Ok(updatedSpot);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteParkingSpot(int id)
        {
            var success = _parkingSpotService.DeleteParkingSpot(id);
            if (!success)
            {
                return NotFound($"Parking spot with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
