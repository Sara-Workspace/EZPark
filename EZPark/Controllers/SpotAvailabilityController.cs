using BLL.Interfaces;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.Extensions;                    // בשביל User.GetUserId()

namespace Parking.Controllers
{
    // אין כאן בדיקות null: אם משהו לא נמצא, הסרוויס זורק שגיאה וה-GlobalExceptionFilter מחזיר 404
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SpotAvailabilityController : ControllerBase
    {
        private readonly ISpotAvailabilityService _availabilityService;

        public SpotAvailabilityController(ISpotAvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult<List<SpotAvailabilityDTO>> GetAllSpotAvailabilities()
        {
            return Ok(_availabilityService.GetAllSpotAvailabilities());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public ActionResult<SpotAvailabilityDTO> GetSpotAvailabilityById(int id)
        {
            return Ok(_availabilityService.GetSpotAvailabilityById(id));
        }

        [AllowAnonymous]
        [HttpGet("spot/{spotId}")]
        public ActionResult<List<SpotAvailabilityDTO>> GetAvailabilityBySpotId(int spotId)
        {
            return Ok(_availabilityService.GetAvailabilityBySpotId(spotId));
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        public ActionResult<SpotAvailabilityDTO> CreateSpotAvailability([FromBody] SpotAvailabilityCreateDTO dto)
        {
            // בדיקת ModelState לא צריך: [ApiController] מחזיר 400 אוטומטית אם ה-DTO לא תקין
            // מזהה בעל החניה מגיע מהטוקן
            var created = _availabilityService.CreateSpotAvailability(dto, User.GetUserId());
            return CreatedAtAction(nameof(GetSpotAvailabilityById), new { id = created.AvailabilityID }, created);
        }

        [Authorize(Roles = "Owner")]
        [HttpDelete("{id}")]
        public IActionResult DeleteSpotAvailability(int id)
        {
            // מוחקים רק מחניה של בעל החניה המחובר
            _availabilityService.DeleteSpotAvailability(id, User.GetUserId());
            return NoContent();
        }
    }
}