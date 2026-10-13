using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    // המחלקה DbContext - שמנהלת את מסד הנתונים בקוד
    // הטיפוס DbSet<T> - שמייצג טבלה במסד הנתונים
    public class ParkingDbContext : DbContext
    {
        public ParkingDbContext(DbContextOptions<ParkingDbContext> options) : base(options)
        {
        }

        // הגדרת הטבלאות במסד הנתונים
        public DbSet<Owners> Owners { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<ParkingSpots> ParkingSpots { get; set; }
        public DbSet<Reservations> Reservations { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<SpotAvailability> SpotAvailability { get; set; }
        public DbSet<DynamicPricingRules> DynamicPricingRules { get; set; }
        public DbSet<Payment> Payment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // מניעת שגיאת Multiple Cascade Paths עבור טבלת הביקורת מול ההזמנות
            modelBuilder.Entity<Reviews>()
                .HasOne(r => r.Reservation)
                .WithMany()
                .HasForeignKey(r => r.ReservationId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}