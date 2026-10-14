using System;
using System.ComponentModel.DataAnnotations;

namespace DAL
{
    public class Reservations // הזמנות
    {
        [Key]
        public int ReservationID { get; set; } // מזהה הזמנה (Primary Key)

        public int SpotID { get; set; } // החניה שהוזמנה
        public int DriverID { get; set; } // הנהג המזמין
        public DateTime StartTime { get; set; } // שעת התחלה בפועל
        public DateTime EndTime { get; set; } // שעת סיום בפועל
        public bool UseCharging { get; set; } // האם הטעין?
        public decimal TotalPrice { get; set; } // מחיר לחישוב סופי
        public string Status { get; set; } = string.Empty; // סטטוס הזמנה
        public DateTime CreatedAt { get; set; } // תאריך יצירת הזמנה

        // Navigation Property

     
        public ParkingSpots ParkingSpot { get; set; } = null!;// קישור חזרה לחניה שהוזמנה


        public Driver Driver { get; set; } = null!; // קישור חזרה לנהג/שוכר שביצע את ההזמנה


        public Payment Payment { get; set; } // קישור לתשלום המשויך להזמנה זו (קשר של אחד לאחד או אחד להרבה)
   
    }

}