namespace BLL.Geocoding
{
    // הממשק: "החוזה" של שירות ההמרה מכתובת למיקום.
    // הוא אומר מה השירות יודע לעשות, בלי לספר איך.
    public interface IGeocodingService
    {
        // מקבל כתובת ועיר, ומחזיר את המיקום על המפה.
        // מחזיר null אם הכתובת לא נמצאה, או אם השירות החיצוני לא ענה
        Task<GeoLocation?> GetLocationAsync(string address, string city, CancellationToken ct = default);
    }
}