using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FuneralOfficeSystem.Models
{
    // Κατηγορία Υπηρεσιών
    public class ServiceCategory
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Το όνομα της κατηγορίας είναι υποχρεωτικό")]
        [Display(Name = "Όνομα Κατηγορίας")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Περιγραφή")]
        public string? Description { get; set; }

        // Ιεραρχική δομή
        public int? ParentCategoryId { get; set; }
        public virtual ServiceCategory? ParentCategory { get; set; }
        public virtual ICollection<ServiceCategory> SubCategories { get; set; } = new List<ServiceCategory>();

        // Σύνδεση με υπηρεσίες και ιδιότητες
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
        public virtual ICollection<ServiceCategoryProperty> Properties { get; set; } = new List<ServiceCategoryProperty>();
    }
}
