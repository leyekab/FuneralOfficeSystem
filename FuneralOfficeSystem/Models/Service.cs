using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FuneralOfficeSystem.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Το όνομα είναι υποχρεωτικό")]
        [StringLength(100)]
        [Display(Name = "Όνομα")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Περιγραφή")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Η κατηγορία είναι υποχρεωτική")]
        [StringLength(50)]
        [Display(Name = "Κατηγορία")]
        public string Category { get; set; } = string.Empty;

        [Display(Name = "Ενεργό")]
        [DefaultValue(true)]
        public bool IsEnabled { get; set; } = true;

        // Navigation properties
        [Display(Name = "Προμηθευτής")]
        public int? SupplierId { get; set; }

        public virtual Supplier? Supplier { get; set; }
        public virtual ICollection<FuneralService> FuneralServices { get; set; } = new List<FuneralService>();
    }
}