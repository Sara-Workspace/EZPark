using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = null!;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // דגל מנהל - ברירת מחדל false
        public bool IsAdmin { get; set; } = false;

        // פרופילים מקושרים (יכולים להיות null אם המשתמש הוא רק בתפקיד אחד)
        public Driver? Driver { get; set; }
        public Owners? Owner { get; set; }
    }
}
