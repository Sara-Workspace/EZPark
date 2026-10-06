using System;
using System.ComponentModel.DataAnnotations;

namespace DAL
{
    public class SpotAvailability // זמינות חניה
    {
        [Key]
        public int AvailabilityId { get; set; } // מזהה ייחודי של רשומת הזמינות (Primary Key)

        public int SpotId { get; set; } // מזהה החניה
        public DateTime StartTime { get; set; } // תאריך התחלה פנוי
        public DateTime EndTime { get; set; } // תאריך סוף פנוי
        public bool IsBooked { get; set; } // האם חלון הזמן הזה כבר הוזמן בפועל?
    }
}