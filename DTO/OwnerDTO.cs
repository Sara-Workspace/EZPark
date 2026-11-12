using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class OwnerDTO
    {
        public int OwnerID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string BankDetails { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsAdmin { get; set; }
    }
}
