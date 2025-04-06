using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FuneralOfficeSystem.Models
{
    public class Deceased
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

        [Required(ErrorMessage = "Ο ΑΜΚΑ είναι υποχρεωτικός")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Ο ΑΜΚΑ πρέπει να έχει ακριβώς 11 ψηφία")]
        public string AMKA { get; set; } = string.Empty;

        [StringLength(9, MinimumLength = 9, ErrorMessage = "Το ΑΦΜ πρέπει να έχει ακριβώς 9 ψηφία")]
        public string AFM { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Ημερομηνία Γέννησης")]
        public DateTime? BirthDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ημερομηνία Θανάτου")]
        public DateTime? DeathDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Τόπος Θανάτου")]
        public string? PlaceOfDeath { get; set; }

        [StringLength(500)]
        [Display(Name = "Σημειώσεις")]
        public string? Notes { get; set; }

        // Navigation properties
        public virtual ICollection<Funeral> Funerals { get; set; } = new List<Funeral>();
    }
}