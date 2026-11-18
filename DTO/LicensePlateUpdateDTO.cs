using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class LicensePlateUpdateDTO
    {
        [Required(ErrorMessage = "מספר רישוי הוא שדה חובה")]
        [StringLength(12, MinimumLength = 5, ErrorMessage = "מספר רישוי לא תקין")]
        public string LicensePlate { get; set; } = string.Empty;
    }
}