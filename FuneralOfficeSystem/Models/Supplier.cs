using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FuneralOfficeSystem.Models
{
    public enum SupplierType
    {
        [Display(Name = "Προϊόντα")]
        Products,
        [Display(Name = "Υπηρεσίες")]
        Services,
        [Display(Name = "Προϊόντα & Υπηρεσίες")]
        Both
    }

    public class Supplier
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Το όνομα είναι υποχρεωτικό")]
        [StringLength(100)]
        [Display(Name = "Όνομα")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Περιγραφή")]
        public string? Description { get; set; }

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

        [Display(Name = "Τύπος Προμηθευτή")]
        public SupplierType SupplierType { get; set; } = SupplierType.Products;

        [Display(Name = "Ενεργός")]
        [DefaultValue(true)]
        public bool IsEnabled { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    }
}