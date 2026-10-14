using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Driver
    {
            [Key]
            public int DriverID { get; set; }// מפתח ראשי - מזהה ייחודי לכל שוכר
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; } // סיסמה מוצפנת להתחברות
            public string Phone { get; set; }
            public decimal CreditBalance { get; set; } // יתרת ארנק דיגיטלי באפליקציה (כפי שמופיע בשרטוט שלך)
            public int PaymentMethodID { get; set; } // מזהה אמצעי התשלום השמור של הנהג
            public DateTime CreatedAt { get; set; }// תאריך ושעת יצירת החשבון

        // רשימת ההזמנות שביצע השוכר הזה (מקושר לטבלת Reservations)
        public List<Reservations> Reservations { get; set; } = new List<Reservations>();

        // רשימת הביקורות שהשוכר הזה כתב (מקושר לטבלת Reviews)
        public List<Reviews> Reviews { get; set; } = new List<Reviews>();

    }
}
