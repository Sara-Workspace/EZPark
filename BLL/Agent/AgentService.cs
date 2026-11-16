using DTO.Chat;
using Microsoft.Extensions.AI;

namespace BLL.Agent
{
    // ה"מוח" של הצ'אט: מכין את השיחה, שולח ל-Gemini ומחזיר את התשובה
    public class AgentService : IAgentService
    {
        // ההוראות ל-AI (הודעת מערכת). רק השרת כותב אותן, הלקוח לא יכול לשנות אותן
        private const string SystemPrompt = """
            אתה עוזר חניות באתר EZPark, אתר להשכרת חניות פרטיות.
            ענה בעברית, בקצרה ובנימוס.
            אל תמציא חניות, כתובות או מחירים. אם אין לך מידע, אמור זאת בכנות.
            """;

        private readonly IChatClient _chatClient;   // ה"טלפון" ל-Gemini, רשום ב-Program.cs (סעיף 6ב)

        public AgentService(IChatClient chatClient)
        {
            _chatClient = chatClient;
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

        public async Task<string> AskAsync(List<ChatMessageDTO> history, CancellationToken ct)
        {
            // שולחים ל-Gemini ומחכים לתשובה (await, כי זה לוקח זמן)
            var response = await _chatClient.GetResponseAsync(BuildMessages(history), cancellationToken: ct);
            return response.Text;
        }
    }
}