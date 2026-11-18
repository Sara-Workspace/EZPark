using System.ComponentModel.DataAnnotations;
using DTO.Validation;
using Microsoft.AspNetCore.Http;

namespace DTO
{
    public class ParkingSpotCreateDTO
    {
        [Required(ErrorMessage = "כתובת היא שדה חובה")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "הכתובת חייבת להכיל בין 2 ל-200 תווים")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "עיר היא שדה חובה")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "שם העיר חייב להכיל בין 2 ל-100 תווים")]
        public string City { get; set; } = string.Empty;

        // לא חובה (? = יכול להיות ריק).
        // ריק  ← השרת ימצא את המיקום לבד לפי הכתובת (OpenStreetMap)
        // מלא ← השרת ישתמש במספרים שנשלחו (למשל כשהכתובת לא קיימת במפה, ובעל החניה סימן על המפה)
        [Range(-90, 90, ErrorMessage = "קו רוחב (Latitude) חייב להיות בין 90- ל-90")]
        public double? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "קו אורך (Longitude) חייב להיות בין 180- ל-180")]
        public double? Longitude { get; set; }

        public bool EvCharger { get; set; }

        [Range(1, 50.0, ErrorMessage = "המחיר לשעה חייב להיות בין 1 ל-50")]
        public decimal BasePricePerHour { get; set; }

        // המדבקות שכתבנו בעצמנו (מהתיקייה DTO/Validation)
        [AllowedExtensions(".jpg", ".jpeg", ".png", ".webp")]
        [MaxFileSize(5)]
        public IFormFile? ImageFile { get; set; } // ? = לא חובה
    }
}