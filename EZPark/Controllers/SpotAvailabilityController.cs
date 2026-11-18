using BLL.Interfaces;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Parking.Controllers
{
    // אין כאן בדיקות null: אם משהו לא נמצא, הסרוויס זורק שגיאה וה-GlobalExceptionFilter מחזיר 404
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SpotAvailabilityController : ControllerBase
    {
        private readonly ISpotAvailabilityService _availabilityService;
        private readonly IOwnerService _ownerService;

        public SpotAvailabilityController(ISpotAvailabilityService availabilityService, IOwnerService ownerService)
        {
            _availabilityService = availabilityService;
            _ownerService = ownerService;
        }

        // מזהה בעל החניה המחובר - לפי האימייל שבטוקן (כמו ב-ParkingSpotsController).
        // לא לפי המספר שבטוקן: אצל משתמש שהוא גם נהג, המספר שבטוקן הוא מספר הנהג שלו
        private int GetCurrentOwnerId()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new UnauthorizedAccessException("לא נמצא אימייל בטוקן.");

            var owner = _ownerService.GetOwnerByEmail(email)
                ?? throw new UnauthorizedAccessException("לא נמצא פרופיל בעל חניה למשתמש.");

            return owner.OwnerID;
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
            // מזהה בעל החניה מגיע מהאימייל שבטוקן
            var created = _availabilityService.CreateSpotAvailability(dto, GetCurrentOwnerId());
            return CreatedAtAction(nameof(GetSpotAvailabilityById), new { id = created.AvailabilityID }, created);
        }

        [Authorize(Roles = "Owner")]
        [HttpDelete("{id}")]
        public IActionResult DeleteSpotAvailability(int id)
        {
            // מוחקים רק מחניה של בעל החניה המחובר
            _availabilityService.DeleteSpotAvailability(id, GetCurrentOwnerId());
            return NoContent();
        }
    }
}