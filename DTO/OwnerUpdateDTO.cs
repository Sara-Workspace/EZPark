using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class OwnerUpdateDTO
    {
        public int OwnerID { get; set; } // נדרש כדי לדעת איזה בעל חניה לעדכן
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string BankDetails { get; set; } = string.Empty;
    }
}
