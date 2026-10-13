using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class PaymentDTO
    {
        public int PaymentID { get; set; }
        public int ReservationID { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethods { get; set; } = string.Empty;
        public string TransactionID { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime PaidAt { get; set; }
    }
}
