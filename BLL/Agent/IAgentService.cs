using DTO.Chat;

namespace BLL.Agent
{
    // הממשק של ה-AI Agent: "החוזה" של מה שהוא יודע לעשות
    public interface IAgentService
    {
        // שאלה ותשובה: מחזיר את כל התשובה בבת אחת
        Task<string> AskAsync(List<ChatMessageDTO> history, CancellationToken ct);

        // הזרמה: מחזיר את התשובה חלק אחרי חלק, ברגע שכל חלק מוכן
        IAsyncEnumerable<string> AskStreamingAsync(List<ChatMessageDTO> history, CancellationToken ct);
    }
}