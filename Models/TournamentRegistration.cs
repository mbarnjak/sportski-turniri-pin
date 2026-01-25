using System.ComponentModel.DataAnnotations;

namespace Turniri.Models
{
    public class TournamentRegistration
    {
        public int Id { get; set; }
        
        public DateTime DatumPrijave { get; set; } = DateTime.Now;
        
        // Foreign keys
        [Required]
        public int TournamentId { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual Tournament? Tournament { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}

