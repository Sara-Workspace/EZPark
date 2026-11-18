using System;
using System.ComponentModel.DataAnnotations;

namespace DAL
{
    // 📒 "דף חשבון" של הארנקים: כל תנועת כסף נרשמת כאן כשורה.
    // ככה תמיד אפשר לראות מאיפה הגיע כל שקל ולאן הלך (וגם כמה האתר הרוויח).
    public class WalletTransaction
    {
        [Key]
        public int WalletTransactionId { get; set; }

        // של מי התנועה. null = של האתר עצמו (העמלה של 10% והקנסות)
        public int? UserId { get; set; }

        // סוג התנועה (ראו WalletTransactionTypes ב-BLL): TopUp / Parking / Earning / PlatformFee / Fine / Withdrawal
        [MaxLength(20)]
        public string Type { get; set; } = string.Empty;

        // הסכום בשקלים. חיובי = נכנס לארנק, שלילי = יצא מהארנק
        public decimal Amount { get; set; }

        // לאיזו חניה (הזמנה) התנועה שייכת, אם בכלל
        public int? ReservationId { get; set; }

        // מספר ההזמנה ב-PayPal (רק בטעינת ארנק). בעזרתו לא מוסיפים את אותו תשלום פעמיים
        [MaxLength(50)]
        public string? PayPalOrderId { get; set; }

        // Pending = טעינה שהתחילה ב-PayPal ועוד לא שולמה. Completed = בוצעה
        [MaxLength(20)]
        public string Status { get; set; } = "Completed";

        // הסבר קצר שמוצג למשתמש ("חניה ברחוב הרצל 10", "טעינת ארנק ב-PayPal")
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
