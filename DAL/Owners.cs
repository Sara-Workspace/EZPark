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

        public int UserId { get; set; } // מפתח זר המקשר לטבלת Users המאוחדת

        public string BankDetails { get; set; } = string.Empty; // פרטי חשבון בנק לקבלת תשלומים

        // ניווט לטבלת המשתמש המאוחדת (מכילה שם, אימייל, סיסמה וטלפון)
        public Users User { get; set; } = null!;

        // רשימת החניות שבבעלות המשכיר הזה (נשאר ללא שינוי!)
        public List<ParkingSpots> ParkingSpots { get; set; } = new List<ParkingSpots>();

    }
}

