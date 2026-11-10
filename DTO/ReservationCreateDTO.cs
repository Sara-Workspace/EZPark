using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ReservationCreateDTO
    {
        public int SpotID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool UseCharging { get; set; }
        public int DriverId { get; set; }
    }
}
