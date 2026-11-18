using System.Text.Json.Serialization;

namespace BLL.Geocoding
{
    // "תבנית" לתוצאה אחת מהתשובה של OpenStreetMap.
    // .NET לוקח את ה-JSON שחזר, ו"יוצק" אותו לתוך המחלקה הזאת.
    // לוקחים רק את השדות שצריך. כל השאר (place_id, osm_type...) לא כתובים כאן, ופשוט מתעלמים מהם
    public class NominatimResult
    {
        // [JsonPropertyName("lat")] = "ב-JSON השדה הזה נקרא lat"
        // שימי לב: OpenStreetMap מחזיר את המספרים כטקסט ("32.06"), ולכן זה string
        [JsonPropertyName("lat")]
        public string Lat { get; set; } = string.Empty;

        [JsonPropertyName("lon")]
        public string Lon { get; set; } = string.Empty;

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = string.Empty;
    }

    // מה שהשירות שלנו מחזיר לשאר הפרויקט: כבר מספרים, נוח לשימוש
    public class GeoLocation
    {
        public double Latitude { get; set; }                     // קו רוחב
        public double Longitude { get; set; }                    // קו אורך
        public string DisplayName { get; set; } = string.Empty;  // הכתובת המלאה כפי שהמפה מצאה
    }
}