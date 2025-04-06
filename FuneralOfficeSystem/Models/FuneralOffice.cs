using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FuneralOfficeSystem.Models
{
    public class FuneralOffice
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Το όνομα είναι υποχρεωτικό")]
        [StringLength(100)]
        [Display(Name = "Όνομα")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Περιγραφή")]
        public string? Description { get; set; }

        [StringLength(200)]
        [Display(Name = "Διεύθυνση")]
        public string? Address { get; set; }

        [StringLength(20)]
        [Display(Name = "Τηλέφωνο")]
        public string? Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Ενεργό")]
        [DefaultValue(true)]
        public bool IsEnabled { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public virtual ICollection<Funeral> Funerals { get; set; } = new List<Funeral>();
    }
}