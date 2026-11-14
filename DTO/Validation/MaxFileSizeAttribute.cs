using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DTO.Validation
{
    // מדבקה משלנו: בודקת שהקובץ לא גדול מהגודל המקסימלי (ב-MB)
    // שימוש: [MaxFileSize(5)]
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxMb; // הגודל המקסימלי ב-MB

        public MaxFileSizeAttribute(int maxMb)
        {
            _maxMb = maxMb;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // file.Length הוא בבתים. 1MB = 1024 * 1024 בתים
            if (value is IFormFile file && file.Length > _maxMb * 1024 * 1024)
                return new ValidationResult($"גודל הקובץ לא יכול לעבור {_maxMb}MB");

            return ValidationResult.Success;
        }
    }
}