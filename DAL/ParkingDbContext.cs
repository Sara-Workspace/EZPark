using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    //ParkingDb-זה גשר בין הקוד למסד נתונים
    //הטיפוס DbSet<T> -שמייצג טבלה במסד הנתונים
    public class ParkingDbContext: DbContext
    {
        public ParkingDbContext(DbContextOptions<ParkingDbContext> options) : base(options)
        {
        }
        // הגדרת הטבלאות במסד הנתונים
        public DbSet<ParkingSpots> ParkingSpots { get; set; }
        public DbSet<Reservations> Reservations { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<SpotAvailability> SpotAvailability { get; set; }
        public DbSet<DynamicPricingRules> DynamicPricingRules { get; set; }
        //שורה זו מורה ל-Entity Framework ליצור טבלה במסד הנתונים עבור הישות Payment ולהתייחס אליה בקוד בתור אוסף של אובייקטים מסוג זה.
        public DbSet<Payment> Payment { get; set; }
    }
}
