using System.ComponentModel.DataAnnotations;

namespace Turniri.ViewModels
{
    public class TournamentViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Naziv turnira je obavezan")]
        [Display(Name = "Naziv turnira")]
        public string Naziv { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Sport je obavezan")]
        [Display(Name = "Sport")]
        public string Sport { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Broj igrača je obavezan")]
        [Display(Name = "Broj igrača")]
        [Range(1, 100, ErrorMessage = "Broj igrača mora biti između 1 i 100")]
        public int BrojIgraca { get; set; }
        
        [Required(ErrorMessage = "Datum je obavezan")]
        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime Datum { get; set; }
        
        [Required(ErrorMessage = "Vrijeme je obavezan")]
        [Display(Name = "Vrijeme")]
        [DataType(DataType.Time)]
        public TimeSpan Vrijeme { get; set; }
        
        // Display properties
        public string? OrganizatorIme { get; set; }
        public int BrojPrijavljenih { get; set; }
        public bool JePun { get; set; }
        public bool JePrijavljen { get; set; }
    }
}

