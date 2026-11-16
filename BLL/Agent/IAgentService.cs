using DTO.Chat;

namespace BLL.Agent
{
    // הממשק של ה-AI Agent: "החוזה" של מה שהוא יודע לעשות
    public interface IAgentService
    {
        // מקבל את כל השיחה עד עכשיו, ומחזיר את התשובה של ה-AI
        Task<string> AskAsync(List<ChatMessageDTO> history, CancellationToken ct);
    }
}