using System.ComponentModel.DataAnnotations;

namespace DAL
{
    public class ParkingSpots // חניות
    {
        [Key]
        public int SpotId { get; set; } // מזהה ייחודי של החניה (Primary Key)

        public int OwnerId { get; set; } // מזהה ייחודי של בעל החניה
        public string Adress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Londitude { get; set; }
        public bool EvChrger { get; set; } // האם יש עמדת הטענה חשמלית?
        public decimal BasePricePerHour { get; set; } // מחיר בסיס לשעה
        public bool IsActive { get; set; } // פעיל?

        // Navigation Property
        public ICollection<DynamicPricingRules> DynamicPricingRules { get; set; } = new List<DynamicPricingRules>();
    }
}