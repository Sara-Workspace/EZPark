using BLL.Interfaces;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.Extensions;

namespace Parking.Controllers
{
    // אין כאן try/catch ואין בדיקות null:
    // אם משהו לא נמצא או לא תקין, הסרוויס זורק שגיאה וה-GlobalExceptionFilter מחזיר את התשובה המתאימה
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
            return Ok(_parkingSpotService.GetAllParkingSpots());
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public ActionResult<ParkingSpotDTO> GetParkingSpotById(int id)
        {
            return Ok(_parkingSpotService.GetParkingSpotById(id));
        }

        [Authorize(Roles = "Owner")]
        [HttpGet("owner/{ownerId}")]
        public ActionResult<List<ParkingSpotDTO>> GetParkingSpotsByOwnerId(int ownerId)
        {
            if (ownerId != User.GetUserId())
            {
                return Forbid();
            }

            return Ok(_parkingSpotService.GetParkingSpotsByOwnerId(ownerId));
        }

        // async: יצירת חניה מחכה ל-API החיצוני (OpenStreetMap) כדי למצוא את המיקום
        // CancellationToken ct: ASP.NET ממלא אותו לבד. אם המשתמש סגר את הדף, הוא "נלחץ"
        [Authorize(Roles = "Owner")]
        [HttpPost]
        public async Task<ActionResult<ParkingSpotDTO>> CreateParkingSpot([FromForm] ParkingSpotCreateDTO dto, CancellationToken ct)
        {
            // בדיקת ModelState לא צריך: [ApiController] מחזיר 400 אוטומטית אם ה-DTO לא תקין
            var createdSpot = await _parkingSpotService.CreateParkingSpotAsync(dto, User.GetUserId(), ct);
            return CreatedAtAction(nameof(GetParkingSpotById), new { id = createdSpot.SpotId }, createdSpot);
        }

        
        [Authorize(Roles = "Admin,Owner")]
        [HttpPut("{id}")]
        public ActionResult<ParkingSpotDTO> UpdateParkingSpot(int id, [FromBody] ParkingSpotDTO dto)
        {
            var updatedSpot = _parkingSpotService.UpdateParkingSpot(
                id,
                dto,
                User.GetUserId(),
                User.IsInRole("Admin"));

            return Ok(updatedSpot);
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpDelete("{id}")]
        public IActionResult DeleteParkingSpot(int id)
        {
            _parkingSpotService.DeleteParkingSpot(
                id,
                User.GetUserId(),
                User.IsInRole("Admin"));

            return NoContent();
        }

    }
}