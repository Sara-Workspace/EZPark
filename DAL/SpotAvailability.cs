using System;
using System.ComponentModel.DataAnnotations;

namespace DAL
{
    // זמינות חניה: "באיזה יום בשבוע ובאילו שעות בעל החניה מוכן להשכיר אותה".
    // כל שורה = חלון זמן אחד שחוזר כל שבוע. למשל: ראשון 08:00-16:00.
    // אפשר כמה שורות לאותו יום (למשל בוקר וגם ערב), ויום בלי שורות = החניה לא מושכרת בו.
    public class SpotAvailability
    {
        [Key]
        public int AvailabilityId { get; set; } // מזהה ייחודי של רשומת הזמינות (Primary Key)

        public int SpotId { get; set; } // מזהה החניה (מפתח זר)

        public DayOfWeek DayOfWeek { get; set; } // יום בשבוע: 0=ראשון ... 6=שבת (נשמר במסד כמספר)
        public TimeSpan StartTime { get; set; }  // שעת התחלה (למשל 08:00)
        public TimeSpan EndTime { get; set; }    // שעת סיום (למשל 16:00)

        // ----- Navigation Property (קשר גומלין) -----
        public ParkingSpots ParkingSpot { get; set; } = null!; // החניה שאליה שייך חלון הזמינות הזה
    }
}
