using CamplusBetaBackend.Data.Entities;
using CamplusBetaBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CamplusBetaBackend.Data {
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid> {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

        public DbSet<ClubEntity> clubs { get; set; }
        public DbSet<EventEntity> events { get; set; }
        public DbSet<HostEntity> hosts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity => {
                entity.ToTable(name: "users");

                entity.Property(e => e.Id).HasColumnName("user_id").HasColumnType("uuid").HasDefaultValueSql("uuid_generate_v4()");
                entity.Property(e => e.UserName).HasColumnName("username");
                entity.Property(e => e.NormalizedUserName).HasColumnName("normalized_username");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.NormalizedEmail).HasColumnName("normalized_email");
                entity.Property(e => e.EmailConfirmed).HasColumnName("email_confirmed");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
                entity.Property(e => e.SecurityStamp).HasColumnName("security_stamp");
                entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrency_stamp");
                entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
                entity.Property(e => e.PhoneNumberConfirmed).HasColumnName("phone_number_confirmed");
                entity.Property(e => e.TwoFactorEnabled).HasColumnName("two_factor_enabled");
                entity.Property(e => e.LockoutEnd).HasColumnName("lockout_end");
                entity.Property(e => e.LockoutEnabled).HasColumnName("lockout_enabled");
                entity.Property(e => e.AccessFailedCount).HasColumnName("access_failed_count");
            });

            modelBuilder.Entity<IdentityUserClaim<Guid>>(entity => {
                entity.ToTable("user_claims");
            });
        }
    }
}
