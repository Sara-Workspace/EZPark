using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class DriverUpdateDTO
    {
        public int DriverID { get; set; } // לא בשימוש: המזהה מגיע מהכתובת ומהטוקן

        [Required(ErrorMessage = "שם פרטי הוא שדה חובה")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "שם פרטי חייב להכיל בין 2 ל-50 תווים")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "שם משפחה הוא שדה חובה")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "שם משפחה חייב להכיל בין 2 ל-50 תווים")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "טלפון הוא שדה חובה")]
        [Phone(ErrorMessage = "מספר הטלפון לא תקין")]
        public string Phone { get; set; } = string.Empty;

        public int PaymentMethodID { get; set; }
    }
}