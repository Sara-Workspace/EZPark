using BLL.PlateRecognition;
using DTO.PlateRecognition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Parking.Controllers
{
    // זיהוי לוחיות רישוי מתמונה.
    // [Authorize] = רק משתמשים מחוברים, כדי שאנשים זרים לא "יבזבזו" לנו את המכסה של Gemini
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlateRecognitionController : ControllerBase
    {
        private readonly IPlateRecognitionService _plateRecognition;

        public PlateRecognitionController(IPlateRecognitionService plateRecognition)
        {
            _plateRecognition = plateRecognition;
        }

        // מקבל תמונה ומחזיר את מספר הרישוי שה-AI קרא.
        // [FromForm] = התמונה מגיעה כקובץ (כמו בהעלאת תמונה של חניה), לא כ-JSON
        [HttpPost("read")]
        public async Task<ActionResult<PlateReadResultDTO>> Read([FromForm] PlateImageDTO dto, CancellationToken ct)
        {
            // מעתיקים את הקובץ לזיכרון, כדי לקבל אותו כמערך של בתים
            using var memory = new MemoryStream();
            await dto.Image.CopyToAsync(memory, ct);

            var plate = await _plateRecognition.ReadPlateAsync(memory.ToArray(), GetMediaType(dto.Image.FileName), ct);

            return Ok(new PlateReadResultDTO { LicensePlate = plate });
        }

        // Gemini צריך לדעת איזה סוג תמונה הוא מקבל. קובעים לפי הסיומת
        // (הסיומת כבר נבדקה ב-DTO, אז מגיעות לכאן רק הסיומות המותרות)
        private static string GetMediaType(string fileName)
        {
            return Path.GetExtension(fileName).ToLower() switch
            {
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "image/jpeg"     // .jpg ו-.jpeg
            };
        }
    }
}