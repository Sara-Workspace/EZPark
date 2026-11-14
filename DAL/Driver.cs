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
        public int DriverID { get; set; } // מפתח ראשי - מזהה ייחודי לכל נהג

        public int UserId { get; set; } // מפתח זר המקשר לטבלת Users המאוחדת

        public decimal CreditBalance { get; set; } // יתרת ארנק דיגיטלי
        public int PaymentMethodID { get; set; } // מזהה אמצעי התשלום השמור

        // ניווט לטבלת המשתמש המאוחדת (מכילה שם, אימייל, סיסמה וטלפון)
        public Users User { get; set; } = null!;

        // רשימת ההזמנות שביצע השוכר הזה (נשאר ללא שינוי!)
        public List<Reservations> Reservations { get; set; } = new List<Reservations>();

        // רשימת הביקורות שהשוכר הזה כתב (נשאר ללא שינוי!)
        public List<Reviews> Reviews { get; set; } = new List<Reviews>();

    }
}
