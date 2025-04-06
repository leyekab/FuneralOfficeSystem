using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FuneralOfficeSystem.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Το όνομα είναι υποχρεωτικό")]
        [StringLength(50)]
        [Display(Name = "Όνομα")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Το επώνυμο είναι υποχρεωτικό")]
        [StringLength(50)]
        [Display(Name = "Επώνυμο")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Ονοματεπώνυμο")]
        public string FullName => $"{FirstName} {LastName}";

        [StringLength(100)]
        [Display(Name = "Σχέση με τον Θανόντα")]
        public string? RelationshipToDeceased { get; set; }

        [StringLength(100)]
        [Display(Name = "Διεύθυνση")]
        public string? Address { get; set; }

        [StringLength(20)]
        [Display(Name = "Τηλέφωνο")]
        public string? Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(9, MinimumLength = 9, ErrorMessage = "Το ΑΦΜ πρέπει να έχει ακριβώς 9 ψηφία")]
        [Display(Name = "ΑΦΜ")]
        public string? AFM { get; set; }

        [StringLength(500)]
        [Display(Name = "Σημειώσεις")]
        public string? Notes { get; set; }

        [Display(Name = "Ενεργός")]
        [DefaultValue(true)]
        public bool IsEnabled { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Funeral> Funerals { get; set; } = new List<Funeral>();
    }
}