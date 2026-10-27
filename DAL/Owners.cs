using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Owners
    {
        [Key]
        public int OwnerID { get; set; } // מפתח ראשי - מזהה ייחודי לכל משכיר
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } = null!; // שינוי שם מ-Password ל-PasswordHash
        public string Role { get; set; } = "Owner";
        public string Phone { get; set; }
        public string BankDetails { get; set; } // פרטי חשבון בנק לקבלת תשלומים
        public DateTime CreatedAt { get; set; }// תאריך ושעת יצירת החשבון

        // רשימת החניות שבבעלות המשכיר הזה (מקושר לטבלת ParkingSpots)
        public List<ParkingSpots> ParkingSpots { get; set; } = new List<ParkingSpots>();

    }
}

