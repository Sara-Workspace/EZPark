using Microsoft.AspNetCore.Http;

namespace BLL.SpotCheck
{
    // תמונה מהמצלמה: הבתים של התמונה + סוג התמונה (למשל "image/jpeg")
    public record CameraSnapshot(byte[] Data, string MediaType);

    // "החוזה" של המצלמה המדומה.
    // אין לנו מצלמות אמיתיות, אז לכל חניה שומרים קובץ אחד: "מה המצלמה רואה עכשיו"
    public interface ICameraService
    {
        void SaveSnapshot(int spotId, IFormFile image); // שומר תמונה חדשה לחניה (דורס את הקודמת)
        CameraSnapshot? GetSnapshot(int spotId);        // מחזיר את התמונה העדכנית (null אם אין)
    }
}