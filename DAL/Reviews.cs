using System;
using System.ComponentModel.DataAnnotations;

namespace DAL
{
    public class Reviews // דירוג וחוות דעת
    {
        [Key]
        public int ReviewId { get; set; } // מזהה ייחודי של הביקורת (Primary Key) - שונה ללשון יחיד

        public int ReservationId { get; set; } // מזהה ההזמנה שעליה נכתבה הביקורת (Foreign Key)

        public int DriverId { get; set; } // מזהה הנהג/השוכר שכתב את הביקורת (Foreign Key לטבלת Driver)

        public int OwnerId { get; set; } // מזהה בעל החניה/המשכיר שקיבל את הביקורת (Foreign Key לטבלת Owner)

        public int Rating { get; set; } // מדד דירוג (למשל מספר שלם בין 1 ל-5)

        public string Comment { get; set; } = string.Empty; // פידבק מילולי חופשי מהנהג

        public DateTime CreatedAt { get; set; } // תאריך ושעת כתיבת הביקורת


        // ----- Navigation Properties (קשרי גומלין) -----

        // קישור ישיר לפרטי ההזמנה המשויכת לביקורת זו
        public Reservations Reservation { get; set; } = null!;

        // קישור ישיר לפרטי הנהג (השוכר) שכתב את הביקורת
        public Driver Driver { get; set; } = null!;

        // קישור ישיר לפרטי בעל החניה (המשכיר) שקיבל את הביקורת
        public Owners Owner { get; set; } = null!;

    }
}