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
    public class SpotAvailabilityController : ControllerBase
    {
        private readonly SpotAvailabilityService _availabilityService;

        public SpotAvailabilityController(SpotAvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult<List<SpotAvailabilityDTO>> GetAllSpotAvailabilities()
        {
            var list = _availabilityService.GetAllSpotAvailabilities();
            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public ActionResult<SpotAvailabilityDTO> GetSpotAvailabilityById(int id)
        {
            var item = _availabilityService.GetSpotAvailabilityById(id);
            if (item == null)
            {
                return NotFound($"Availability record with ID {id} not found.");
            }
            return Ok(item);
        }

        [AllowAnonymous]
        [HttpGet("spot/{spotId}")]
        public ActionResult<List<SpotAvailabilityDTO>> GetAvailabilityBySpotId(int spotId)
        {
            var list = _availabilityService.GetAvailabilityBySpotId(spotId);
            return Ok(list);
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        public ActionResult<SpotAvailabilityDTO> CreateSpotAvailability([FromBody] SpotAvailabilityCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // ← תיקון 4: מזהה בעל החניה מהטוקן
            var created = _availabilityService.CreateSpotAvailability(dto, User.GetUserId());
            if (created == null)
            {
                return NotFound($"Parking spot with ID {dto.SpotID} not found.");
            }
            return CreatedAtAction(nameof(GetSpotAvailabilityById), new { id = created.AvailabilityID }, created);
        }

        [Authorize(Roles = "Owner")]
        [HttpDelete("{id}")]
        public IActionResult DeleteSpotAvailability(int id)
        {
            // ← תיקון 4: מוחקים רק מחניה של בעל החניה המחובר
            var success = _availabilityService.DeleteSpotAvailability(id, User.GetUserId());
            if (!success)
            {
                return NotFound($"Availability record with ID {id} not found.");
            }
            return NoContent();
        }
    }
}