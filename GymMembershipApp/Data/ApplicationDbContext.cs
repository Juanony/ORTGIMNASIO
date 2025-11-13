using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GymMembershipApp.Models;

namespace GymMembershipApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<MembershipPlan> MembershipPlans { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Member>()
                .HasOne(m => m.MembershipPlan)
                .WithMany(mp => mp.Members)
                .HasForeignKey(m => m.MembershipPlanId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Member)
                .WithMany(m => m.Attendances)
                .HasForeignKey(a => a.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Member)
                .WithMany(m => m.Payments)
                .HasForeignKey(p => p.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.MembershipPlan)
                .WithMany()
                .HasForeignKey(p => p.MembershipPlanId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure decimal precision
            modelBuilder.Entity<MembershipPlan>()
                .Property(mp => mp.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            // Seed data
            modelBuilder.Entity<MembershipPlan>().HasData(
                new MembershipPlan
                {
                    Id = 1,
                    Name = "Monthly",
                    Description = "Access to gym for 30 days",
                    Price = 50.00m,
                    DurationInDays = 30,
                    IsActive = true
                },
                new MembershipPlan
                {
                    Id = 2,
                    Name = "Quarterly",
                    Description = "Access to gym for 90 days with 10% discount",
                    Price = 135.00m,
                    DurationInDays = 90,
                    IsActive = true
                },
                new MembershipPlan
                {
                    Id = 3,
                    Name = "Annual",
                    Description = "Access to gym for 365 days with 20% discount",
                    Price = 480.00m,
                    DurationInDays = 365,
                    IsActive = true
                }
            );
        }
    }
}
