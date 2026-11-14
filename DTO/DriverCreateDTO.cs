using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class DriverCreateDTO
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
<<<<<<< HEAD
        public int? PaymentMethodID { get; set; }
=======

        public int PaymentMethodID { get; set; }
>>>>>>> 89367d3fe84d3a208dfdabdabdfacb95610d2724
    }
}