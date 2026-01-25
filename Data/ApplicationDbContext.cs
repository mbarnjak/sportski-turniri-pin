using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Turniri.Models;

namespace Turniri.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentRegistration> TournamentRegistrations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Tournament
            builder.Entity<Tournament>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Organizator)
                    .WithMany(u => u.CreatedTournaments)
                    .HasForeignKey(e => e.OrganizatorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure TournamentRegistration
            builder.Entity<TournamentRegistration>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Tournament)
                    .WithMany(t => t.Registrations)
                    .HasForeignKey(e => e.TournamentId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Registrations)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Prevent duplicate registrations
                entity.HasIndex(e => new { e.TournamentId, e.UserId })
                    .IsUnique();
            });
        }
    }
}

