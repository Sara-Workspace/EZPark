using System.ComponentModel.DataAnnotations;
using DTO.Validation;
using Microsoft.AspNetCore.Http;

namespace DTO.PlateRecognition
{
    // מה הלקוח שולח: תמונה של רכב
    public class PlateImageDTO
    {
        // אותן מדבקות כמו בתמונה של חניה (מהתיקייה DTO/Validation)
        [Required(ErrorMessage = "יש לצרף תמונה של הרכב")]
        [AllowedExtensions(".jpg", ".jpeg", ".png", ".webp")]
        [MaxFileSize(5)]
        public IFormFile Image { get; set; } = null!;
    }
}