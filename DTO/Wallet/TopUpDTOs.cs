using System.ComponentModel.DataAnnotations;

namespace DTO.Wallet
{
    // שלב 1 בטעינה: כמה כסף לטעון
    public class TopUpCreateDTO
    {
        [Range(10, 1000, ErrorMessage = "אפשר לטעון בין 10 ל-1,000 ₪ בכל פעם")]
        public decimal Amount { get; set; }
    }

    // מה השרת מחזיר בשלב 1: מספר ההזמנה ש-PayPal יצר, והכתובת של דף התשלום ב-PayPal
    public class TopUpOrderDTO
    {
        public string OrderId { get; set; } = string.Empty;
        public string ApproveUrl { get; set; } = string.Empty;   // לשם הדפדפן עובר כדי לשלם
    }

    // שלב 2 בטעינה: "שילמתי ב-PayPal, תבדקו ותוסיפו לי"
    public class TopUpCaptureDTO
    {
        [Required(ErrorMessage = "חסר מספר הזמנה של PayPal")]
        [StringLength(50)]
        public string OrderId { get; set; } = string.Empty;
    }

    // משיכת כסף (בעל חניה)
    public class WithdrawDTO
    {
        [Range(1, 100000, ErrorMessage = "סכום המשיכה חייב להיות לפחות 1 ₪")]
        public decimal Amount { get; set; }
    }

    // למנהל: כמה האתר הרוויח
    public class PlatformSummaryDTO
    {
        public decimal TotalFees { get; set; }    // סך עמלות 10%
        public int ParkingCount { get; set; }     // מכמה חניות
        public decimal TotalFines { get; set; }   // סך קנסות
    }
}
