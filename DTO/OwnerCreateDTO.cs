using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class OwnerCreateDTO
    {
        [Required(ErrorMessage = "שם פרטי הוא שדה חובה")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "שם משפחה הוא שדה חובה")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "מייל הוא שדה חובה")]
        [EmailAddress(ErrorMessage = "כתובת המייל לא תקינה")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "סיסמה היא שדה חובה")]
        [MinLength(8, ErrorMessage = "הסיסמה חייבת להכיל לפחות 8 תווים")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "טלפון הוא שדה חובה")]
        [Phone(ErrorMessage = "מספר הטלפון לא תקין")]
        public string Phone { get; set; } = string.Empty;

        public string BankDetails { get; set; } = string.Empty;
    }
}