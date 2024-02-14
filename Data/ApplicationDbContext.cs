using CamplusBetaBackend.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CamplusBetaBackend.Data {
    public class ApplicationDbContext : DbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
        }

        public DbSet<ClubEntity> clubs { get; set; }
        public DbSet<EventEntity> events { get; set; }
        public DbSet<HostEntity> hosts { get; set; }
    }
}
