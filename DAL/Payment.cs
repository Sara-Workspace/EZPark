using System;
using System.ComponentModel.DataAnnotations;

namespace DAL
{
    public class Payment // תשלומים
    {
        [Key]
        public int PaymentID { get; set; } // מזהה תשלום (Primary Key)

        public int ReservationID { get; set; } // מזהה הזמנה
        public decimal Amount { get; set; } // סכום לתשלום
        public string PaymentMethods { get; set; } = string.Empty; // אמצעי תשלום
        public int TransactionID { get; set; } // מזהה עסקה
        public bool Status { get; set; } // סטטוס תשלום
        public DateTime PaidAt { get; set; } // תאריך ושעת ביצוע

        // Navigation Property
        public Reservations Reservation { get; set; } = null!;
    }
}