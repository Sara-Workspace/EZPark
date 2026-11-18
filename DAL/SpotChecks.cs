using System.ComponentModel.DataAnnotations;

namespace DAL
{
    // בדיקות חניה: כל פעם שה-AI בודק איזה רכב עומד בחניה, נשמרת כאן שורה.
    // מתי בודקים? כשנהג מדווח "החניה תפוסה", כשבעל החניה מעלה תמונה, או כשהמנהל מבקש בדיקה מהמצלמה
    public class SpotChecks
    {
        [Key]
        public int SpotCheckId { get; set; } // מזהה הבדיקה (Primary Key)

        public int SpotId { get; set; } // איזו חניה נבדקה

        [MaxLength(10)]
        public string LicensePlate { get; set; } = string.Empty; // המספר שה-AI קרא (רק ספרות)

        public DateTime CheckedAt { get; set; } // מתי בדקו (UTC)

        [MaxLength(20)]
        public string Source { get; set; } = string.Empty; // מי ביקש: Driver / Owner / Admin

        [MaxLength(200)]
        public string RequestedByEmail { get; set; } = string.Empty; // האימייל של מי שביקש

        [MaxLength(20)]
        public string Result { get; set; } = string.Empty; // התוצאה: Paying / OwnerCar / OtherDriver / Unknown

        public int? FinedUserId { get; set; } // מי קיבל קנס (null = אף אחד)
        public int FinePoints { get; set; } // כמה נקודות קנס (0 = בלי קנס)

        // Navigation Property
        public ParkingSpots ParkingSpot { get; set; } = null!; // קישור לחניה שנבדקה
    }
}