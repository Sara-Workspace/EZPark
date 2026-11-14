using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ParkingSpotDetailedDTO
    {
        public int SpotId { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal BasePricePerHour { get; set; }
        public bool EvCharger { get; set; }
        public string? Image { get; set; } // שם הקובץ


        public OwnerDTO OwnerInfo { get; set; } = null!;
    }
}
