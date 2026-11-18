namespace BLL.Payments
{
    // שיחה עם PayPal (סביבת הבדיקות - Sandbox)
    public interface IPayPalService
    {
        // שלב 1: "PayPal, תפתח הזמנה על X ₪". מחזיר את מספר ההזמנה + הכתובת של דף התשלום
        Task<PayPalCreatedOrder> CreateOrderAsync(decimal amount, CancellationToken ct = default);

        // שלב 2: "PayPal, תגבה את ההזמנה". מחזיר כמה באמת נגבה (או זורק שגיאה אם לא שולם)
        Task<decimal> CaptureOrderAsync(string orderId, CancellationToken ct = default);
    }
}
