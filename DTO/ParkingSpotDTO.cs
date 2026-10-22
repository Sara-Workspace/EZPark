using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ParkingSpotDTO
    {
        public int SpotId { get; set; }
        public int OwnerId { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool EvCharger { get; set; }
        public decimal BasePricePerHour { get; set; }
        public bool IsActive { get; set; }
        public string? Image { get; set; } // שם הקובץ
    }
}
