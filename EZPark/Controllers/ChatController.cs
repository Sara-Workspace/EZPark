using BLL.Agent;
using DTO.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Parking.Controllers
{
    // הצ'אט עם ה-AI.
    // [Authorize] = רק משתמשים מחוברים, כדי שאנשים זרים לא "יבזבזו" לנו את המכסה של Gemini
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IAgentService _agentService;

        public ChatController(IAgentService agentService)
        {
            _agentService = agentService;
        }

        // שאלה ותשובה: מקבל את כל השיחה, ומחזיר את התשובה של ה-AI
        // CancellationToken ct: אם המשתמש סגר את הדף באמצע, מפסיקים לחכות ל-Gemini
        [HttpPost]
        public async Task<IActionResult> Ask([FromBody] ChatRequestDTO dto, CancellationToken ct)
        {
            // בדיקת ModelState לא צריך: [ApiController] מחזיר 400 אוטומטית אם ה-DTO לא תקין
            var answer = await _agentService.AskAsync(dto.Messages, ct);
            return Ok(new { text = answer });
        }
    }
}