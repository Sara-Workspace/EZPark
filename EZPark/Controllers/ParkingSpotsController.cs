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
        [HttpGet("{id}")]
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

        [Authorize(Roles = "Owner")]
        [HttpPost]
        public ActionResult<ParkingSpotDTO> CreateParkingSpot([FromForm] ParkingSpotCreateDTO dto)
        {
            // בדיקת ModelState לא צריך: [ApiController] מחזיר 400 אוטומטית אם ה-DTO לא תקין
            var createdSpot = _parkingSpotService.CreateParkingSpot(dto, User.GetUserId());
            return CreatedAtAction(nameof(GetParkingSpotById), new { id = createdSpot.SpotId }, createdSpot);
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpPut("{id}")]
        public ActionResult<ParkingSpotDTO> UpdateParkingSpot(int id, [FromBody] ParkingSpotDTO dto)
        {
            return Ok(_parkingSpotService.UpdateParkingSpot(id, dto, User.GetUserId()));
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpDelete("{id}")]
        public IActionResult DeleteParkingSpot(int id)
        {
            _parkingSpotService.DeleteParkingSpot(id, User.GetUserId());
            return NoContent();
        }
    }
}