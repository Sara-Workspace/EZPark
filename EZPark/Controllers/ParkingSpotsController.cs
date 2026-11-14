using BLL.Services;
using BLL.Interfaces;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.Extensions;

namespace Parking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ParkingSpotsController : ControllerBase
    {
        private readonly IParkingSpotService _parkingSpotService;

        public ParkingSpotsController(IParkingSpotService parkingSpotService)
        {
            _parkingSpotService = parkingSpotService;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult<List<ParkingSpotDTO>> GetAllParkingSpots()
        {
            var spots = _parkingSpotService.GetAllParkingSpots();
            return Ok(spots);
        }

        [AllowAnonymous]
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

        [Authorize(Roles = "Owner")]
        [HttpGet("owner/{ownerId}")]
        public ActionResult<List<ParkingSpotDTO>> GetParkingSpotsByOwnerId(int ownerId)
        {
            if (ownerId != User.GetUserId())
            {
                return Forbid();
            }

            var spots = _parkingSpotService.GetParkingSpotsByOwnerId(ownerId);
            return Ok(spots);
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        public ActionResult<ParkingSpotDTO> CreateParkingSpot([FromForm] ParkingSpotCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdSpot = _parkingSpotService.CreateParkingSpot(dto, User.GetUserId());
                return CreatedAtAction(nameof(GetParkingSpotById), new { id = createdSpot.SpotId }, createdSpot);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpPut("{id}")]
        public ActionResult<ParkingSpotDTO> UpdateParkingSpot(int id, [FromBody] ParkingSpotDTO dto)
        {
            var updatedSpot = _parkingSpotService.UpdateParkingSpot(id, dto, User.GetUserId());
            if (updatedSpot == null)
            {
                return NotFound($"Parking spot with ID {id} not found or access denied.");
            }
            return Ok(updatedSpot);
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpDelete("{id}")]
        public IActionResult DeleteParkingSpot(int id)
        {
            var success = _parkingSpotService.DeleteParkingSpot(id, User.GetUserId());
            if (!success)
            {
                return NotFound($"Parking spot with ID {id} not found or access denied.");
            }
            return NoContent();
        }
      
    }
}
    
