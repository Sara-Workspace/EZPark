using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DTO
{
    public class ParkingSpotCreateDTO
    {
        public int OwnerId { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool EvCharger { get; set; }
        public decimal BasePricePerHour { get; set; }
        public IFormFile? ImageFile { get; set; } // הקובץ עצמו
    }
}
