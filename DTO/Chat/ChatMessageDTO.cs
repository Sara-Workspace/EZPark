using System.ComponentModel.DataAnnotations;

namespace DTO.Chat
{
    // הודעה אחת בשיחה עם ה-AI
    public class ChatMessageDTO
    {
        // מי כתב את ההודעה: user = המשתמש, assistant = ה-AI.
        // RegularExpression בודק שהערך הוא בדיוק אחת משתי המילים האלה, ולא שום דבר אחר
        [Required(ErrorMessage = "חובה לציין מי כתב את ההודעה")]
        [RegularExpression("^(user|assistant)$", ErrorMessage = "Role חייב להיות user או assistant")]
        public string Role { get; set; } = string.Empty;

        // תוכן ההודעה
        [Required(ErrorMessage = "תוכן ההודעה הוא שדה חובה")]
        [MaxLength(2000, ErrorMessage = "ההודעה ארוכה מדי (עד 2000 תווים)")]
        public string Text { get; set; } = string.Empty;
    }
}