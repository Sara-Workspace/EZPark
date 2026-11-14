using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class PaymentCreateDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "יש לבחור הזמנה")]
        public int ReservationID { get; set; }

        [Range(0.01, 100000.0, ErrorMessage = "הסכום חייב להיות בין 0.01 ל-100,000")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "אמצעי תשלום הוא שדה חובה")]
        [StringLength(50, ErrorMessage = "אמצעי התשלום ארוך מדי")]
        public string PaymentMethods { get; set; } = string.Empty;

        [Required(ErrorMessage = "מספר עסקה הוא שדה חובה")]
        [StringLength(100, ErrorMessage = "מספר העסקה ארוך מדי")]
        public string TransactionID { get; set; } = string.Empty;
    }
}