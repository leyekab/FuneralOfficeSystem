using System.ComponentModel.DataAnnotations;

namespace FuneralOfficeSystem.Models
{
    public class ProductCategory
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Το όνομα της κατηγορίας είναι υποχρεωτικό")]
        [Display(Name = "Όνομα Κατηγορίας")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Περιγραφή")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Display(Name = "Ενεργή")]
        public bool IsEnabled { get; set; } = true;

        // Ιεραρχική δομή
        [Display(Name = "Γονική Κατηγορία")]
        public int? ParentCategoryId { get; set; }
        public virtual ProductCategory? ParentCategory { get; set; }
        public virtual ICollection<ProductCategory> SubCategories { get; set; } = new List<ProductCategory>();

        // Σύνδεση με προϊόντα και ιδιότητες
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<CategoryProperty> Properties { get; set; } = new List<CategoryProperty>();
    }
}