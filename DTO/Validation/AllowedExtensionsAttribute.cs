using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DTO.Validation
{
    // מדבקה משלנו: בודקת שסוג הקובץ (הסיומת) הוא אחד מהמותרים
    // שימוש: [AllowedExtensions(".jpg", ".png")]
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions; // רשימת הסיומות המותרות

        public AllowedExtensionsAttribute(params string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // value is IFormFile file = "אם הערך הוא קובץ, תכניס אותו למשתנה file"
            if (value is IFormFile file)
            {
                var ext = Path.GetExtension(file.FileName).ToLower();
                if (!_extensions.Contains(ext))
                    return new ValidationResult($"מותר להעלות רק קבצים מסוג: {string.Join(", ", _extensions)}");
            }

            // אין קובץ? מבחינתנו זה תקין. אם הוא חובה, זה התפקיד של [Required]
            return ValidationResult.Success;
        }
    }
}