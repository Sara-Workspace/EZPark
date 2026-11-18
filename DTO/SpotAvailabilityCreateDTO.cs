using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class SpotAvailabilityCreateDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "יש לבחור חניה")]
        public int SpotID { get; set; }

        // בודק שהערך הוא יום אמיתי (0=ראשון ... 6=שבת), ולא למשל 9
        [EnumDataType(typeof(DayOfWeek), ErrorMessage = "יום בשבוע לא תקין")]
        public DayOfWeek DayOfWeek { get; set; }

        // "סיום אחרי התחלה" הוא כלל עסקי, ייבדק ב-BLL בצעד 3
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}