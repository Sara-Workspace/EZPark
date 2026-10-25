using Microsoft.AspNetCore.Http;

namespace BLL.Services
{
    // המימוש: איך שירות התמונות עושה את העבודה
    public class ImageService : IImageService
    {
        private readonly string _folder; // הנתיב לתיקיית התמונות

        public ImageService(string folder)
        {
            _folder = folder;
        }

        public string Save(IFormFile file)
        {
            // בדיקת סוג הקובץ - רק תמונות
            var ext = Path.GetExtension(file.FileName).ToLower();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowed.Contains(ext))
                throw new ArgumentException("מותר להעלות רק jpg, png או webp");

            // בדיקת גודל - עד 5MB
            if (file.Length > 5 * 1024 * 1024)
                throw new ArgumentException("הקובץ גדול מדי");

            // שם חדש וייחודי, לא השם שהמשתמש נתן
            var fileName = Guid.NewGuid() + ext;

            // שמירת הקובץ בתיקייה
            using (var stream = File.Create(Path.Combine(_folder, fileName)))
            {
                file.CopyTo(stream);
            }

            return fileName; // מחזירים רק את השם
        }

        public void Delete(string fileName)
        {
            var path = Path.Combine(_folder, fileName);
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}