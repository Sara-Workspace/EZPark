using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class OwnerUpdateDTO
    {
        public int OwnerID { get; set; } // לא בשימוש: המזהה מגיע מהכתובת ומהטוקן

        [Required(ErrorMessage = "שם פרטי הוא שדה חובה")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "שם פרטי חייב להכיל בין 2 ל-50 תווים")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "שם משפחה הוא שדה חובה")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "שם משפחה חייב להכיל בין 2 ל-50 תווים")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "טלפון הוא שדה חובה")]
        [Phone(ErrorMessage = "מספר הטלפון לא תקין")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "פרטי חשבון בנק הם שדה חובה")]
        [StringLength(100, ErrorMessage = "פרטי חשבון הבנק ארוכים מדי")]
        public string BankDetails { get; set; } = string.Empty;
    }
}