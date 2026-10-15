using BLL.Services;
using DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Parking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotAvailabilityController : ControllerBase
    {
        private readonly SpotAvailabilityService _availabilityService;

        public SpotAvailabilityController(SpotAvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        [HttpGet]
        public ActionResult<List<SpotAvailabilityDTO>> GetAllSpotAvailabilities()
        {
            var list = _availabilityService.GetAllSpotAvailabilities();
            return Ok(list);
        }

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

        [HttpGet("spot/{spotId}")]
        public ActionResult<List<SpotAvailabilityDTO>> GetAvailabilityBySpotId(int spotId)
        {
            var list = _availabilityService.GetAvailabilityBySpotId(spotId);
            return Ok(list);
        }

        [HttpPost]
        public ActionResult<SpotAvailabilityDTO> CreateSpotAvailability([FromBody] SpotAvailabilityCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = _availabilityService.CreateSpotAvailability(dto);
            // שים לב: אם ב-SpotAvailabilityDTO שדה המזהה הוא AvailabilityID או Id, ודא שהוא תואם:
            return CreatedAtAction(nameof(GetSpotAvailabilityById), new { id = created.AvailabilityID }, created);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSpotAvailability(int id)
        {
            var success = _availabilityService.DeleteSpotAvailability(id);
            if (!success)
            {
                return NotFound($"Availability record with ID {id} not found.");
            }
            return NoContent();
        }
    }
}
