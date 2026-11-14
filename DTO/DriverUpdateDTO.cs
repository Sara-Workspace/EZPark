using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class DriverUpdateDTO
    {
        public int DriverID { get; set; } // נדרש כדי לדעת איזה נהג לעדכן
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int PaymentMethodID { get; set; }
    }
}
