using System.ComponentModel;
using BLL.Geocoding;
using BLL.Services;
using DTO;

namespace BLL.Agent
{
    // סיכום קצר של חניה בשביל ה-AI.
    // מחזירים מעט: כל מה שהכלי מחזיר נשלח למודל, וזה עולה מהמכסה
    public record SpotSummary(int Id, string Address, string City, double DistanceKm, decimal PricePerHour, bool EvCharger);

    // הכלים של ה-AI. המודל לא רואה את הקוד, רק את השם, ה-[Description] והפרמטרים,
    // ולפיהם הוא מחליט מתי להשתמש בכל כלי.
    // כל הכלים כאן רק קוראים מידע. כלים שמשנים נתונים (כמו הזמנה) לא נותנים למודל.
    // שימו לב: כל הפרמטרים הם חובה (בלי ? ובלי ברירת מחדל), כי Gemini לא תומך בפרמטרים אופציונליים בצורה הזאת
    public class ParkingTools
    {
        private readonly IGeocodingService _geocoding;        // ה-API החיצוני (OpenStreetMap)
        private readonly SpotSearchService _spotSearch;       // חיפוש החניות במסד

        public ParkingTools(IGeocodingService geocoding, SpotSearchService spotSearch)
        {
            _geocoding = geocoding;
            _spotSearch = spotSearch;
        }

        // כלי 1: כתובת ← מיקום (פונה ל-API חיצוני, ולכן async)
        [Description("ממיר כתובת בישראל למיקום על המפה (קו רוחב וקו אורך). מחזיר null אם הכתובת לא נמצאה.")]
        public async Task<GeoLocation?> FindLocationAsync(
            [Description("רחוב ומספר בית, למשל: הרצל 10")] string address,
            [Description("שם העיר, למשל: תל אביב")] string city,
            CancellationToken ct)
        {
            return await _geocoding.GetLocationAsync(address, city, ct);
        }

        // כלי 2: מיקום ← חניות פנויות קרובות מהמסד (משתמש בחיפוש שהחברה בנתה)
        [Description("מחפש חניות פנויות ופעילות באתר ליד מיקום. מחזיר עד 5 חניות ממוינות, עם מרחק בק\"מ ומחיר לשעה בשקלים.")]
        public List<SpotSummary> SearchParkingSpots(
            [Description("קו רוחב של היעד")] double latitude,
            [Description("קו אורך של היעד")] double longitude,
            [Description("רדיוס חיפוש בק\"מ. אם המשתמש לא ביקש, שלח 2")] double radiusKm,
            [Description("true אם צריך רק חניות עם עמדת טעינה לרכב חשמלי, אחרת false")] bool evOnly,
            [Description("מחיר מקסימלי לשעה בשקלים. אם המשתמש לא הגביל מחיר, שלח 0")] double maxPricePerHour,
            [Description("מיון: smart (מרחק ומחיר יחד), distance (הכי קרוב), price (הכי זול). אם המשתמש לא ביקש, שלח smart")] string sortBy)
        {
            var request = new SpotSearchRequestDTO
            {
                Latitude = latitude,
                Longitude = longitude,
                RadiusKm = Math.Clamp(radiusKm, 0.1, 50),   // שומרים על הטווח המותר, גם אם המודל ביקש משהו אחר
                EvOnly = evOnly,
                SortBy = string.IsNullOrWhiteSpace(sortBy) ? "smart" : sortBy,
                Limit = 20
            };

            // 0 (או פחות) = בלי הגבלת מחיר
            decimal? maxPrice = maxPricePerHour > 0 ? (decimal)maxPricePerHour : null;

            return _spotSearch.Search(request)
                .Where(s => maxPrice == null || s.EffectivePricePerHour <= maxPrice)
                .Take(5)
                .Select(s => new SpotSummary(s.SpotId, s.Address, s.City, s.DistanceKm, s.EffectivePricePerHour, s.EvCharger))
                .ToList();
        }
    }
}