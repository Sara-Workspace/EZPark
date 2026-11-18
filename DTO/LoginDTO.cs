using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "מייל הוא שדה חובה")]
        [EmailAddress(ErrorMessage = "כתובת המייל לא תקינה")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "סיסמה היא שדה חובה")]
        public string Password { get; set; } = string.Empty;
    }
}