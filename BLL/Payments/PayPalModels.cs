using System.Text.Json.Serialization;

namespace BLL.Payments
{
    // "תבניות" לתשובות של PayPal (כמו NominatimResult ב-Geocoding).
    // .NET לוקח את ה-JSON שחזר, ו"יוצק" אותו לתוך המחלקות האלה.
    // לוקחים רק את השדות שצריך, וכל השאר פשוט מתעלמים מהם

    // תשובה לבקשת "כרטיס כניסה" (access token)
    public class PayPalTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
    }

    // תשובה על הזמנה (גם ביצירה וגם בגבייה)
    public class PayPalOrderResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;          // מספר ההזמנה ב-PayPal

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;      // למשל COMPLETED = שולם

        [JsonPropertyName("links")]
        public List<PayPalLink> Links { get; set; } = new();    // קישורים, ביניהם דף התשלום

        [JsonPropertyName("purchase_units")]
        public List<PayPalPurchaseUnit> PurchaseUnits { get; set; } = new();
    }

    public class PayPalLink
    {
        [JsonPropertyName("href")]
        public string Href { get; set; } = string.Empty;        // הכתובת

        [JsonPropertyName("rel")]
        public string Rel { get; set; } = string.Empty;         // מה היא. "payer-action" = הדף שבו המשתמש משלם
    }

    public class PayPalPurchaseUnit
    {
        [JsonPropertyName("payments")]
        public PayPalPayments? Payments { get; set; }
    }

    public class PayPalPayments
    {
        [JsonPropertyName("captures")]
        public List<PayPalCapture> Captures { get; set; } = new();   // הגביות שבוצעו
    }

    public class PayPalCapture
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public PayPalAmount Amount { get; set; } = new();
    }

    public class PayPalAmount
    {
        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; } = string.Empty;  // ILS = שקל

        // PayPal מחזיר את הסכום כטקסט ("50.00"), כמו ש-OpenStreetMap מחזיר את המיקום כטקסט
        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;
    }

    // מה שהשירות שלנו מחזיר לשאר הפרויקט אחרי יצירת הזמנה
    public class PayPalCreatedOrder
    {
        public string OrderId { get; set; } = string.Empty;      // מספר ההזמנה
        public string ApproveUrl { get; set; } = string.Empty;   // לאן לשלוח את המשתמש כדי לשלם
    }
}
