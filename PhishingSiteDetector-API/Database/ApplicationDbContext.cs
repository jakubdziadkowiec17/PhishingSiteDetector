using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PhishingSiteDetector_API.Models.Entities;

namespace PhishingSiteDetector_API.Database
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<DataSet> DataSets { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<SiteLog> SiteLogs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.Language)
                .WithMany(b => b.ApplicationUsers)
                .HasForeignKey(a => a.LanguageCode)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DataSet>()
                .HasOne(a => a.ApplicationUser)
                .WithMany(b => b.DataSets)
                .HasForeignKey(a => a.CreationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(a => a.ApplicationUser)
                .WithMany(b => b.RefreshTokens)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}