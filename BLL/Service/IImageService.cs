using Microsoft.AspNetCore.Http;

namespace BLL.Services
{
    // החוזה: מה שירות התמונות יודע לעשות
    public interface IImageService
    {
        string Save(IFormFile file);   // שומר קובץ ומחזיר את השם החדש שלו
        void Delete(string fileName);  // מוחק קובץ לפי השם
    }
}