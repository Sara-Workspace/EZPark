using System.ComponentModel.DataAnnotations;

namespace DAL
{
    public class ParkingSpots // חניות
    {
        [Key]
        public int SpotId { get; set; } // מזהה ייחודי של החניה (Primary Key)

        public int OwnerId { get; set; } // מזהה ייחודי של בעל החניה
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool EvCharger { get; set; } // האם יש עמדת הטענה חשמלית?
        public decimal BasePricePerHour { get; set; } // מחיר בסיס לשעה
        public bool IsActive { get; set; } // פעיל?
        public string? Image { get; set; } // שם קובץ התמונה בלבד

        // Navigation Property


        public Owners Owner { get; set; } = null!;// קישור חזרה למשכיר (הבעלים של החניה)
        public ICollection<DynamicPricingRules> DynamicPricingRules { get; set; } = new List<DynamicPricingRules>();
    }
}