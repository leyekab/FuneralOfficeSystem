using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuneralOfficeSystem.Models
{
    public class Product
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
        public bool IsEnabled { get; set; } = true;

        [Display(Name = "Κατηγορία")]
        public int CategoryId { get; set; }
        public virtual ProductCategory Category { get; set; } = null!;

        [Display(Name = "Προμηθευτής")]
        public int? SupplierId { get; set; }
        public virtual Supplier? Supplier { get; set; }

        [Display(Name = "Τελευταία Τροποποίηση")]
        public DateTime? LastModifiedAt { get; set; }

        [Display(Name = "Τροποποιήθηκε από")]
        [StringLength(256)]
        public string? LastModifiedBy { get; set; }

        public virtual ICollection<ProductProperty> Properties { get; set; } = new List<ProductProperty>();
        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public virtual ICollection<FuneralProduct> FuneralProducts { get; set; } = new List<FuneralProduct>();
    }
}