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
        public DbSet<Users> Users { get; set; }
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

            // 1. מניעת כפילות אימיילים בטבלת Users המאוחדת
            modelBuilder.Entity<Users>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // 2. הגדרת יחס אחד-לאחד מול Driver
            modelBuilder.Entity<Driver>()
                .HasOne(d => d.User)
                .WithOne(u => u.Driver)
                .HasForeignKey<Driver>(d => d.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // 3. הגדרת יחס אחד-לאחד מול Owners
            modelBuilder.Entity<Owners>()
                .HasOne(o => o.User)
                .WithOne(u => u.Owner)
                .HasForeignKey<Owners>(o => o.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // 4. מניעת שגיאת Multiple Cascade Paths עבור טבלת הביקורות מול ההזמנות (נשאר כרגיל)
            modelBuilder.Entity<Reviews>()
                .HasOne(r => r.Reservation)
                .WithMany()
                .HasForeignKey(r => r.ReservationId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}