using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace BLL.Geocoding
{
    // כאן השרת שלנו הוא ה"לקוח": הוא פונה ל-API של OpenStreetMap ושואל "איפה הכתובת הזאת?"
    public class GeocodingService : IGeocodingService
    {
        private readonly HttpClient _http;                       // "הדפדפן" של השרת. מגיע מוכן מ-AddHttpClient ב-Program.cs
        private readonly ILogger<GeocodingService> _logger;      // "יומן" לרישום תקלות (רק בשבילנו, המפתחות)

        public GeocodingService(HttpClient http, ILogger<GeocodingService> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<GeoLocation?> GetLocationAsync(string address, string city, CancellationToken ct = default)
        {
            // 1. בונים את הכתובת לבקשה, בדיוק כמו זו שהדבקת בדפדפן.
            //    EscapeDataString מקודד רווחים ועברית, כדי שהכתובת תעבור תקין באינטרנט
            //    countrycodes=il = לחפש רק בישראל
            var query = Uri.EscapeDataString($"{address}, {city}");
            var url = $"search?q={query}&format=json&limit=1&countrycodes=il";

            try
            {
                // 2. שולחים GET ומחכים לתשובה.
                //    .NET הופך את ה-JSON שחזר לרשימה של NominatimResult (התבנית מקובץ 1)
                var results = await _http.GetFromJsonAsync<List<NominatimResult>>(url, ct);

                // 3. רשימה ריקה = הכתובת לא נמצאה
                var first = results?.FirstOrDefault();
                if (first == null)
                    return null;

                // 4. ממירים את הטקסט ("32.06") למספרים (32.06).
                //    InvariantCulture = לקרוא נקודה עשרונית, בלי קשר להגדרות השפה של המחשב
                if (!double.TryParse(first.Lat, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) ||
                    !double.TryParse(first.Lon, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
                    return null;

                // 5. מחזירים מיקום מסודר (המחלקה השנייה מקובץ 1)
                return new GeoLocation
                {
                    Latitude = lat,
                    Longitude = lon,
                    DisplayName = first.DisplayName
                };
            }
            catch (HttpRequestException ex)      // השרת שלהם החזיר שגיאה, או שאין חיבור לאינטרנט
            {
                _logger.LogWarning(ex, "Geocoding API failed: {Status}", ex.StatusCode);
                return null;
            }
            catch (TaskCanceledException)        // עברו 10 שניות (Timeout) בלי תשובה
            {
                _logger.LogWarning("Geocoding API timed out");
                return null;
            }
            catch (JsonException ex)             // התשובה הגיעה במבנה לא צפוי (ה-API לא "שלנו", הוא יכול להשתנות)
            {
                _logger.LogWarning(ex, "Geocoding API returned unexpected JSON");
                return null;
            }
        }
    }
}