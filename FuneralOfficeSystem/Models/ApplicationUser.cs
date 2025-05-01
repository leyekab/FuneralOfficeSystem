using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FuneralOfficeSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // Προσθήκη νέων πεδίων
        [PersonalData]
        [Display(Name = "Ημερομηνία Δημιουργίας")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [PersonalData]
        [Display(Name = "Ενεργός")]
        public bool IsEnabled { get; set; } = true;

        // Προαιρετικά: Προσθήκη computed property για πλήρες όνομα
        [Display(Name = "Ονοματεπώνυμο")]
        public string FullName => $"{FirstName} {LastName}";
    }
}