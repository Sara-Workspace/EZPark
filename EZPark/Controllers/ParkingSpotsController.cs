using BLL.Interfaces;
using BLL.Service;
using BLL.Services;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.Extensions;
using DAL.Repositories;

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
        private readonly SpotSearchService _spotSearchService;
        private readonly SpotHoldService _spotHoldService;
        private readonly IOwnersRepository _ownerRepo;

        public ParkingSpotsController(IParkingSpotService parkingSpotService, SpotSearchService spotSearchService, SpotHoldService spotHoldService, IOwnersRepository ownerRepo)
        {
            _parkingSpotService = parkingSpotService;
            _spotSearchService = spotSearchService;
            _spotHoldService = spotHoldService;
            _ownerRepo = ownerRepo;
        }

        private int GetCurrentOwnerId()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                ?? throw new UnauthorizedAccessException("לא נמצא אימייל בטוקן.");

            var owner = _ownerRepo.GetOwnerByEmail(email)
                ?? throw new UnauthorizedAccessException("לא נמצא פרופיל בעל חניה למשתמש.");

            return owner.OwnerID;
        }
       // [Authorize(Roles = "Owner")]
        [HttpGet("my")]
        public ActionResult<List<ParkingSpotDTO>> GetMyParkingSpots()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                ?? throw new UnauthorizedAccessException("לא נמצא אימייל בטוקן.");

            var owner = _ownerRepo.GetOwnerByEmail(email);
            if (owner == null)
            {
                return Ok(new List<ParkingSpotDTO>());
            }

            return Ok(_parkingSpotService.GetParkingSpotsByOwnerId(owner.OwnerID));
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult<List<ParkingSpotDTO>> GetAllParkingSpots()
        {
            return Ok(_parkingSpotService.GetAllParkingSpots());
        }
        [HttpGet("search")]
        public ActionResult<List<ParkingSpotSearchResultDTO>> SearchParkingSpots([FromQuery] SpotSearchRequestDTO request)
        {
            // ה-Hold רשום לפי DriverID, ולכן גם החיפוש צריך להשוות לפי DriverID (ולא לפי המזהה מהטוקן)
            int? driverId = null;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (email != null)
            {
                driverId = _spotHoldService.GetDriverId(email);
            }

            var results = _spotSearchService.Search(request, driverId);
            return Ok(results);
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
            // בודקים לפי האימייל שבטוקן (GetCurrentOwnerId), כדי שיעבוד גם למשתמש שהוא גם נהג
            if (ownerId != GetCurrentOwnerId())
            {
                return Forbid();
            }

            return Ok(_parkingSpotService.GetParkingSpotsByOwnerId(ownerId));
        }

        // async: יצירת חניה מחכה ל-API החיצוני (OpenStreetMap) כדי למצוא את המיקום
        // CancellationToken ct: ASP.NET ממלא אותו לבד. אם המשתמש סגר את הדף, הוא "נלחץ"
        
        [HttpPost]
        public async Task<ActionResult<ParkingSpotDTO>> CreateParkingSpot([FromForm] ParkingSpotCreateDTO dto, CancellationToken ct)
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? throw new UnauthorizedAccessException("לא נמצא אימייל בטוקן.");
            // בדיקת ModelState לא צריך: [ApiController] מחזיר 400 אוטומטית אם ה-DTO לא תקין
            var createdSpot = await _parkingSpotService.CreateParkingSpotAsync(dto, email, ct);
            return CreatedAtAction(nameof(GetParkingSpotById), new { id = createdSpot.SpotId }, createdSpot);
        }

        
      //  [Authorize(Roles = "Admin,Owner")]
        [HttpPut("{id}")]
        public ActionResult<ParkingSpotDTO> UpdateParkingSpot(int id, [FromBody] ParkingSpotDTO dto)
        {
            var isAdmin = User.IsInRole("Admin");
            var ownerId = isAdmin ? 0 : GetCurrentOwnerId();

            var updatedSpot = _parkingSpotService.UpdateParkingSpot(id, dto, ownerId, isAdmin);
            return Ok(updatedSpot);
        }

       // [Authorize(Roles = "Admin,Owner")]
        [HttpDelete("{id}")]
        public IActionResult DeleteParkingSpot(int id)
        {
            var isAdmin = User.IsInRole("Admin");
            var ownerId = isAdmin ? 0 : GetCurrentOwnerId();

            _parkingSpotService.DeleteParkingSpot(id, ownerId, isAdmin);
            return NoContent();
        }
        [HttpPost("{id:int}/hold")]
        public ActionResult<SpotHoldResultDTO> HoldSpot(int id)
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? throw new UnauthorizedAccessException("לא נמצא אימייל בטוקן.");

            var result = _spotHoldService.HoldSpot(id, email);
            if (result == null)
            {
                return Conflict(new { message = "החניה כבר נתפסה על ידי נהג אחר או שאינה זמינה." });
            }
            return Ok(result);
        }

        [HttpDelete("{id:int}/hold")]
        public IActionResult ReleaseSpot(int id)
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
         ?? throw new UnauthorizedAccessException("לא נמצא אימייל בטוקן.");

            _spotHoldService.ReleaseSpot(id, email);
            return NoContent();
        }

    }
}