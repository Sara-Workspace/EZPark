using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DynamicPricingRules//כללי תמחור דינמי
    {
        public int Id { get; set; }//מזהה כלל
        public int SpotID { get; set; }//מזהה חניה
        public DateTime StartHour { get; set; }//שעת התחלת עומס
        public DateTime EndtHour { get; set; }//שעת סיום עומס
        public decimal PriceMultiplier { get; set; }//הכפלת המחיר בשעת עומס
        public decimal EvExtraCharge { get; set; }//תוספת לטעינה


        // Navigation Property
        public ParkingSpots ParkingSpot { get; set; } = null!;
    }
}
