using System.Runtime.CompilerServices;
using DTO.Chat;
using Microsoft.Extensions.AI;

namespace BLL.Agent
{
    // ה"מוח" של הצ'אט: מכין את השיחה, מצרף את הכלים, שולח ל-Gemini ומחזיר את התשובה
    public class AgentService : IAgentService
    {
        // ההוראות ל-AI (הודעת מערכת). רק השרת כותב אותן, הלקוח לא יכול לשנות אותן
        private const string SystemPrompt = """
            אתה עוזר חניות באתר EZPark, אתר להשכרת חניות פרטיות.
            ענה בעברית, בקצרה ובנימוס.
            כשמבקשים חניה ליד כתובת: קודם מצא את המיקום עם FindLocationAsync, ואז חפש חניות עם SearchParkingSpots.
            הצג רק חניות שהכלים החזירו. אל תמציא חניות, כתובות או מחירים.
            המחירים הם בשקלים לשעה. אם לא נמצאו חניות, אמור זאת והצע להרחיב את רדיוס החיפוש.
            אתה לא יכול להזמין חניה בעצמך. כדי להזמין, המשתמש בוחר חניה באתר.
            """;

        private readonly IChatClient _chatClient;     // ה"טלפון" ל-Gemini (Program.cs, סעיף 6ב)
        private readonly ParkingTools _parkingTools;  // הכלים

        public AgentService(IChatClient chatClient, ParkingTools parkingTools)
        {
            _chatClient = chatClient;
            _parkingTools = parkingTools;
        }

        // בונה את רשימת ההודעות ל-Gemini: קודם ההוראות, ואחריהן כל השיחה מהלקוח
        private List<ChatMessage> BuildMessages(List<ChatMessageDTO> history)
        {
            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, SystemPrompt)
            };

            foreach (var m in history)
            {
                // ממירים את התווית שלנו (user / assistant) לתווית של הספרייה
                var role = m.Role == "assistant" ? ChatRole.Assistant : ChatRole.User;
                messages.Add(new ChatMessage(role, m.Text));
            }

            return messages;
        }

        // ה"תפריט" של הכלים שנשלח למודל.
        // AIFunctionFactory.Create קורא את השם, ה-[Description] והפרמטרים של כל מתודה, ובונה מהם תיאור למודל
        private ChatOptions BuildOptions() => new ChatOptions
        {
            Tools = new List<AITool>
            {
                AIFunctionFactory.Create(_parkingTools.FindLocationAsync),
                AIFunctionFactory.Create(_parkingTools.SearchParkingSpots)
            }
        };

        // שאלה ותשובה: מחכים עד שהמודל מסיים, ומחזירים הכל בבת אחת
        public async Task<string> AskAsync(List<ChatMessageDTO> history, CancellationToken ct)
        {
            // אם המודל מבקש כלי, UseFunctionInvocation (מ-Program.cs) מריץ אותו ושולח את התוצאה בחזרה, עד שמגיעה תשובה בטקסט
            var response = await _chatClient.GetResponseAsync(BuildMessages(history), BuildOptions(), ct);
            return response.Text;
        }

        // הזרמה: מחזירים כל חלק של התשובה ברגע שהוא מגיע מהמודל
        // [EnumeratorCancellation] = אם המשתמש סגר את הדף, גם ההזרמה נעצרת
        public async IAsyncEnumerable<string> AskStreamingAsync(
            List<ChatMessageDTO> history,
            [EnumeratorCancellation] CancellationToken ct)
        {
            var updates = _chatClient.GetStreamingResponseAsync(BuildMessages(history), BuildOptions(), ct);

            await foreach (var update in updates)
            {
                // חלק מהעדכונים הם בקשות לכלים, בלי טקסט. מדלגים עליהם
                if (!string.IsNullOrEmpty(update.Text))
                    yield return update.Text;   // "הנה עוד חלק", ושולחים אותו מיד
            }
        }
    }
}