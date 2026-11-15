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
        // נקודות לשימוש בחניה (כל משתמש חדש מקבל 3000)
        public int Points { get; set; } = 3000;

        // מתי הנקודות התחדשו לאחרונה
        public DateTime PointsResetAt { get; set; } = DateTime.UtcNow;

       

        // פרופילים מקושרים (יכולים להיות null אם המשתמש הוא רק בתפקיד אחד)
        public Driver? Driver { get; set; }
        public Owners? Owner { get; set; }
    }
}
