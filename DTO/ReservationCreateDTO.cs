using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class ReservationCreateDTO
    {
        // על int לא שמים [Required] (תמיד יש ערך, לפחות 0). [Range] פוסל גם את 0
        [Range(1, int.MaxValue, ErrorMessage = "יש לבחור חניה")]
        public int SpotID { get; set; }

        // "סיום אחרי התחלה" הוא כלל עסקי, ייבדק ב-BLL בצעד 3
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public bool UseCharging { get; set; }
        // אין כאן DriverId. הנהג נלקח מהטוקן
    }
}