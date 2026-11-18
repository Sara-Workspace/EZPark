using DTO.SpotCheck;
using Microsoft.AspNetCore.Http;

namespace BLL.SpotCheck
{
    // "החוזה" של בדיקת החניות
    public interface ISpotCheckService
    {
        // מנהל: מעלה "מה המצלמה רואה עכשיו" בחניה (הדמיה של מצלמה)
        void SaveCameraSnapshot(int spotId, IFormFile image);

        // נהג: הגיע לחניה והיא תפוסה. בודקים לפי תמונת המצלמה
        Task<SpotCheckResultDTO> ReportOccupiedAsync(int spotId, string driverEmail, CancellationToken ct);

        // בעל חניה: מעלה תמונה כדי לוודא שהרכב בחניה שלו משלם לו
        Task<SpotCheckResultDTO> OwnerCheckAsync(int spotId, string ownerEmail, IFormFile image, CancellationToken ct);

        // מנהל: בודק לפי תמונת המצלמה אם הרכב בחניה משלם
        Task<SpotCheckResultDTO> AdminCheckAsync(int spotId, string adminEmail, CancellationToken ct);
    }
}