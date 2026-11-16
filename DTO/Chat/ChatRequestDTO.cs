using System.ComponentModel.DataAnnotations;

namespace DTO.Chat
{
    // מה ש-React שולח לשרת: כל ההודעות בשיחה עד עכשיו.
    // ההודעה האחרונה ברשימה היא השאלה החדשה
    public class ChatRequestDTO
    {
        [Required(ErrorMessage = "חובה לשלוח הודעות")]
        [MinLength(1, ErrorMessage = "חובה לשלוח לפחות הודעה אחת")]
        public List<ChatMessageDTO> Messages { get; set; } = new();
    }
}