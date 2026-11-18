using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BLL.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BLL.Payments
{
    // כאן השרת שלנו הוא ה"לקוח": הוא פונה ל-API של PayPal (בדיוק כמו GeocodingService מול OpenStreetMap).
    // חשוב: הבדיקה שהתשלום עבר נעשית כאן בשרת, מול PayPal ישירות.
    // לא סומכים על הדפדפן, כי משתמש יכול לשנות אותו ולכתוב "שילמתי" בלי לשלם.
    public class PayPalService : IPayPalService
    {
        private readonly HttpClient _http;                 // "הדפדפן" של השרת. מגיע מוכן מ-AddHttpClient ב-Program.cs
        private readonly ILogger<PayPalService> _logger;   // "יומן" לרישום תקלות (רק בשבילנו, המפתחות)
        private readonly string _clientId;                 // המזהה של האפליקציה שלנו ב-PayPal
        private readonly string _secret;                   // הסוד (כמו סיסמה). רק ממשתנה סביבה!
        private readonly string _currency;                 // ILS = שקל
        private readonly string _returnUrl;                // לאן PayPal מחזיר את המשתמש אחרי שילם
        private readonly string _cancelUrl;                // לאן PayPal מחזיר אם ביטל

        public PayPalService(HttpClient http, IConfiguration config, ILogger<PayPalService> logger)
        {
            _http = http;
            _logger = logger;
            // המפתחות מגיעים ממשתני הסביבה PayPal__ClientId ו-PayPal__Secret (לא מ-appsettings ולא מגיט!)
            _clientId = config["PayPal:ClientId"] ?? string.Empty;
            _secret = config["PayPal:Secret"] ?? string.Empty;
            _currency = config["PayPal:Currency"] ?? "ILS";
            _returnUrl = config["PayPal:ReturnUrl"] ?? "http://localhost:5173/wallet?paypal=return";
            _cancelUrl = config["PayPal:CancelUrl"] ?? "http://localhost:5173/wallet?paypal=cancel";
        }

        // 1. "כרטיס כניסה" זמני ל-PayPal: שולחים מזהה + סוד, ומקבלים access token.
        //    אחר כך שמים אותו בכותרת Authorization של כל בקשה (כמו הטוקן שלנו ב-Swagger: "Bearer ...")
        private async Task LoginToPayPalAsync(CancellationToken ct)
        {
            // אם המפתחות לא הוגדרו במחשב הזה - הודעה ברורה במקום קריסה
            if (string.IsNullOrWhiteSpace(_clientId) || string.IsNullOrWhiteSpace(_secret))
                throw new BusinessRuleException("התשלום ב-PayPal עדיין לא מוגדר במחשב הזה (חסרים PayPal__ClientId ו-PayPal__Secret).");

            // "Basic" = מזהה:סוד, מקודד ב-Base64 (כך PayPal דורש)
            var idAndSecret = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_secret}"));
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", idAndSecret);

            var form = new FormUrlEncodedContent(new Dictionary<string, string> { ["grant_type"] = "client_credentials" });
            var response = await _http.PostAsync("v1/oauth2/token", form, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("PayPal login failed: {Status}", response.StatusCode);
                throw new BusinessRuleException("לא הצלחנו להתחבר ל-PayPal. בדקו שהמפתחות של PayPal נכונים.");
            }

            var token = await response.Content.ReadFromJsonAsync<PayPalTokenResponse>(ct);
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.AccessToken);
        }

        public async Task<PayPalCreatedOrder> CreateOrderAsync(decimal amount, CancellationToken ct = default)
        {
            try
            {
                await LoginToPayPalAsync(ct);

                // 2. מה שולחים: "הזמנה על X שקלים, ואחרי התשלום תחזירו את המשתמש לאתר שלנו".
                //    הסכום נשלח כטקסט עם נקודה ("50.00"). InvariantCulture = נקודה, בלי קשר להגדרות המחשב
                var order = new
                {
                    intent = "CAPTURE",
                    purchase_units = new[]
                    {
                        new
                        {
                            description = "EZPark - טעינת ארנק",
                            amount = new { currency_code = _currency, value = amount.ToString("0.00", CultureInfo.InvariantCulture) }
                        }
                    },
                    payment_source = new
                    {
                        paypal = new
                        {
                            experience_context = new
                            {
                                brand_name = "EZPark",
                                user_action = "PAY_NOW",            // בדף של PayPal הכפתור יהיה "שלם עכשיו"
                                shipping_preference = "NO_SHIPPING", // אין משלוח, זה ארנק
                                return_url = _returnUrl,
                                cancel_url = _cancelUrl
                            }
                        }
                    }
                };

                var response = await _http.PostAsJsonAsync("v2/checkout/orders", order, ct);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("PayPal create order failed: {Status}", response.StatusCode);
                    throw new BusinessRuleException("PayPal לא הצליח לפתוח את התשלום. נסו שוב בעוד רגע.");
                }

                // 3. מהתשובה לוקחים: מספר ההזמנה + הקישור לדף התשלום ("payer-action")
                var created = await response.Content.ReadFromJsonAsync<PayPalOrderResponse>(ct);
                var payLink = created!.Links.FirstOrDefault(l => l.Rel == "payer-action")
                    ?? throw new BusinessRuleException("PayPal לא החזיר קישור לתשלום.");

                return new PayPalCreatedOrder { OrderId = created.Id, ApproveUrl = payLink.Href };
            }
            catch (HttpRequestException ex)      // אין חיבור לאינטרנט / ל-PayPal
            {
                _logger.LogWarning(ex, "PayPal API failed");
                throw new BusinessRuleException("אין חיבור ל-PayPal כרגע. בדקו את האינטרנט ונסו שוב.");
            }
            catch (TaskCanceledException)        // עברו 20 שניות בלי תשובה
            {
                throw new BusinessRuleException("PayPal לא ענה בזמן. נסו שוב.");
            }
            catch (JsonException ex)             // התשובה הגיעה במבנה לא צפוי
            {
                _logger.LogWarning(ex, "PayPal returned unexpected JSON");
                throw new BusinessRuleException("התקבלה תשובה לא צפויה מ-PayPal.");
            }
        }

        public async Task<decimal> CaptureOrderAsync(string orderId, CancellationToken ct = default)
        {
            try
            {
                await LoginToPayPalAsync(ct);

                // 4. "תגבו את ההזמנה". EscapeDataString: שמשתמש לא יוכל "להשתיל" משהו בכתובת דרך מספר ההזמנה
                var response = await _http.PostAsJsonAsync($"v2/checkout/orders/{Uri.EscapeDataString(orderId)}/capture", new { }, ct);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("PayPal capture failed: {Status}", response.StatusCode);
                    throw new BusinessRuleException("התשלום ב-PayPal לא הושלם, ולא ירד ממך כסף. נסו שוב.");
                }

                // 5. בודקים מה PayPal ענה: האם באמת שולם, ובאיזה מטבע וכמה
                var result = await response.Content.ReadFromJsonAsync<PayPalOrderResponse>(ct);
                if (result!.Status != "COMPLETED")
                    throw new BusinessRuleException("PayPal עדיין לא אישר את התשלום.");

                var capture = result.PurchaseUnits.FirstOrDefault()?.Payments?.Captures.FirstOrDefault()
                    ?? throw new BusinessRuleException("PayPal לא החזיר את פרטי התשלום.");

                if (capture.Amount.CurrencyCode != _currency)
                    throw new BusinessRuleException("התשלום התקבל במטבע לא נכון.");

                // "50.00" -> 50.00
                return decimal.Parse(capture.Amount.Value, CultureInfo.InvariantCulture);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "PayPal API failed");
                throw new BusinessRuleException("אין חיבור ל-PayPal כרגע. בדקו את האינטרנט ונסו שוב.");
            }
            catch (TaskCanceledException)
            {
                throw new BusinessRuleException("PayPal לא ענה בזמן. נסו שוב.");
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "PayPal returned unexpected JSON");
                throw new BusinessRuleException("התקבלה תשובה לא צפויה מ-PayPal.");
            }
        }
    }
}
