using System.Text.Json;
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

        // שאלה ותשובה (בלי הזרמה). נוח לבדיקות ב-Swagger
        [HttpPost]
        public async Task<IActionResult> Ask([FromBody] ChatRequestDTO dto, CancellationToken ct)
        {
            var answer = await _agentService.AskAsync(dto.Messages, ct);
            return Ok(new { text = answer });
        }

        // הזרמה: React יקרא לכתובת הזאת, והתשובה תופיע מילה אחרי מילה.
        // הפורמט נקרא Server-Sent Events: כל חלק הוא שורה שמתחילה ב-"data:" ונגמרת בשורה ריקה
        [HttpPost("stream")]
        public async Task Stream([FromBody] ChatRequestDTO dto, CancellationToken ct)
        {
            Response.ContentType = "text/event-stream";     // "אני שולח הזרמה, לא JSON רגיל"
            Response.Headers.CacheControl = "no-cache";     // "אל תשמור את זה בזיכרון המטמון"

            try
            {
                await foreach (var chunk in _agentService.AskStreamingAsync(dto.Messages, ct))
                {
                    // עוטפים ב-JSON, כדי ששורה חדשה בתוך הטקסט לא תשבור את הפורמט
                    var json = JsonSerializer.Serialize(new { text = chunk });
                    await Response.WriteAsync($"data: {json}\n\n", ct);
                    await Response.Body.FlushAsync(ct);    // שולחים עכשיו, לא מחכים
                }

                await Response.WriteAsync("data: [DONE]\n\n", ct);   // "סיימתי"
            }
            catch (Exception ex) when (Response.HasStarted && ex is not OperationCanceledException)
            {
                // התשובה כבר התחילה להישלח, ולכן השומר (GlobalExceptionFilter) כבר לא יכול לשנות אותה.
                // אז שולחים הודעת שגיאה בתוך ההזרמה עצמה
                var json = JsonSerializer.Serialize(new { title = "התשובה נקטעה. נסו שוב." });
                await Response.WriteAsync($"event: error\ndata: {json}\n\n");
            }
        }
    }
}