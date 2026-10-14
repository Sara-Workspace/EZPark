using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class DriverDTO
    {
        public int DriverID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public decimal CreditBalance { get; set; }
        public int PaymentMethodID { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
