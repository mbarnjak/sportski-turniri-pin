using Microsoft.AspNetCore.Identity;

namespace Turniri.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Ime { get; set; }
        public DateTime DatumRegistracije { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual ICollection<Tournament> CreatedTournaments { get; set; } = new List<Tournament>();
        public virtual ICollection<TournamentRegistration> Registrations { get; set; } = new List<TournamentRegistration>();
    }
}

