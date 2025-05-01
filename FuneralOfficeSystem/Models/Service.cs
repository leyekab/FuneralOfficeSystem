using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Display(Name = "Ενεργό")]
        [DefaultValue(true)]
        public bool IsEnabled { get; set; } = true;

        [Display(Name = "Τιμή")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Display(Name = "Έκπτωση %")]
        [Range(0, 100)]
        public decimal? DiscountPercentage { get; set; }

        // Navigation properties
        public int CategoryId { get; set; }
        public virtual ServiceCategory Category { get; set; } = null!;

        public int? SupplierId { get; set; }
        public virtual Supplier? Supplier { get; set; }

        // Δυναμικές ιδιότητες υπηρεσίας
        public virtual ICollection<ServiceProperty> Properties { get; set; } = new List<ServiceProperty>();
        public virtual ICollection<FuneralService> FuneralServices { get; set; } = new List<FuneralService>();
    }
}