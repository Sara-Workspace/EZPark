using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ParkingSpotWithReviewsDTO
    {
        public int SpotId { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal BasePricePerHour { get; set; }
        public bool EvCharger { get; set; }
        public bool IsActive { get; set; }

        // מעבר ל-100%: רשימת כל התגובות והדירוגים של החניה הזו
        public List<ReviewDTO> Reviews { get; set; } = new List<ReviewDTO>();
    }
}
