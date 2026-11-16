using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class SpotHoldResultDTO
    {
        public int SpotId { get; set; }
        public DateTime HeldUntilUtc { get; set; }
        public int ExpiresInSeconds { get; set; }
    }
}
