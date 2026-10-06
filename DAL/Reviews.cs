using System;
using System.ComponentModel.DataAnnotations;

namespace DAL
{
    public class Reviews // דירוג וחוות דעת
    {
        [Key]
        public int ReviewsID { get; set; } // מזהה חוות דעת (Primary Key)

        public int ReservationID { get; set; } // מזהה הזמנה שעליה הדירוג
        public int DriverID { get; set; } // מזהה כותב החוויה-המדרג
        public int OwnerId { get; set; } // מזהה בעל החניה
        public int Rating { get; set; } // דירוג 1-5
        public string Comment { get; set; } = string.Empty; // טקסט חופשי
        public DateTime CreatedAt { get; set; } // תאריך ושעת כתיבת החוויה
    }
}