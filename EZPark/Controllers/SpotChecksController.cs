using System.Security.Claims;
using BLL.SpotCheck;
using DTO.PlateRecognition;
using DTO.SpotCheck;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Parking.Controllers
{
    // בדיקת חניות עם קורא הלוחיות (AI).
    // לכל פעולה יש [Authorize(Roles = ...)] משלה: מי מותר לו לעשות אותה
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SpotChecksController : ControllerBase
    {
        private readonly ISpotCheckService _spotCheckService;

        public SpotChecksController(ISpotCheckService spotCheckService)
        {
            _spotCheckService = spotCheckService;
        }

        // מנהל: מעלה "מה המצלמה רואה עכשיו" בחניה (הדמיה של מצלמה).
        // PUT כי זה מחליף את התמונה הקודמת של אותה חניה
        [Authorize(Roles = "Admin")]
        [HttpPut("{spotId:int}/camera")]
        public IActionResult UploadCameraSnapshot(int spotId, [FromForm] PlateImageDTO dto)
        {
            _spotCheckService.SaveCameraSnapshot(spotId, dto.Image);
            return NoContent(); // 204 = "הצליח, ואין מה להחזיר"
        }

        // נהג: הגיע לחניה והיא תפוסה. השרת בודק לפי תמונת המצלמה
        [Authorize(Roles = "Driver")]
        [HttpPost("{spotId:int}/occupied")]
        public async Task<ActionResult<SpotCheckResultDTO>> ReportOccupied(int spotId, CancellationToken ct)
        {
            var result = await _spotCheckService.ReportOccupiedAsync(spotId, GetEmail(), ct);
            return Ok(result);
        }

        // בעל חניה: מעלה תמונה של הרכב שבחניה שלו
        [Authorize(Roles = "Owner")]
        [HttpPost("{spotId:int}/owner-check")]
        public async Task<ActionResult<SpotCheckResultDTO>> OwnerCheck(int spotId, [FromForm] PlateImageDTO dto, CancellationToken ct)
        {
            var result = await _spotCheckService.OwnerCheckAsync(spotId, GetEmail(), dto.Image, ct);
            return Ok(result);
        }

        // מנהל: בודק לפי תמונת המצלמה אם הרכב בחניה משלם
        [Authorize(Roles = "Admin")]
        [HttpPost("{spotId:int}/admin-check")]
        public async Task<ActionResult<SpotCheckResultDTO>> AdminCheck(int spotId, CancellationToken ct)
        {
            var result = await _spotCheckService.AdminCheckAsync(spotId, GetEmail(), ct);
            return Ok(result);
        }

        // האימייל של המשתמש המחובר, מתוך הטוקן (כמו בקונטרולרים של שרה)
        private string GetEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new UnauthorizedAccessException("לא נמצא אימייל בטוקן.");
        }
    }
}