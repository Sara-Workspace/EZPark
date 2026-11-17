using Microsoft.AspNetCore.Http;

namespace BLL.SpotCheck
{
    // המצלמה המדומה.
    // במערכת אמיתית המצלמה בחניה הייתה מעלה תמונה לבד כל כמה שניות.
    // אצלנו המנהל מעלה אותה במקומה, וכל חניה מקבלת קובץ אחד: spot-5.jpg, spot-12.png...
    public class CameraService : ICameraService
    {
        private readonly string _folder; // התיקייה של תמונות המצלמה (נקבעת ב-Program.cs)

        public CameraService(string folder)
        {
            _folder = folder;
            Directory.CreateDirectory(_folder); // יוצר את התיקייה אם היא עוד לא קיימת
        }

        public void SaveSnapshot(int spotId, IFormFile image)
        {
            // מוחקים תמונה קודמת של אותה חניה (אולי עם סיומת אחרת, למשל png במקום jpg)
            foreach (var oldFile in Directory.GetFiles(_folder, $"spot-{spotId}.*"))
                File.Delete(oldFile);

            var ext = Path.GetExtension(image.FileName).ToLower();
            var path = Path.Combine(_folder, $"spot-{spotId}{ext}");

            using var stream = File.Create(path);
            image.CopyTo(stream);
        }

        public CameraSnapshot? GetSnapshot(int spotId)
        {
            // מחפשים קובץ שמתחיל ב-"spot-5." (לא משנה מה הסיומת)
            var path = Directory.GetFiles(_folder, $"spot-{spotId}.*").FirstOrDefault();
            if (path == null)
                return null; // עדיין לא העלו תמונה לחניה הזאת

            return new CameraSnapshot(File.ReadAllBytes(path), GetMediaType(path));
        }

        // Gemini צריך לדעת איזה סוג תמונה הוא מקבל. קובעים לפי הסיומת.
        // public static = גם SpotCheckService משתמש בזה (לתמונה שבעל החניה מעלה)
        public static string GetMediaType(string fileName)
        {
            return Path.GetExtension(fileName).ToLower() switch
            {
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "image/jpeg"     // .jpg ו-.jpeg
            };
        }
    }
}