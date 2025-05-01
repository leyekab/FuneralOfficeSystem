using System.ComponentModel.DataAnnotations;

namespace FuneralOfficeSystem.Models
{
    // Ιδιότητες Κατηγορίας Υπηρεσιών
    public class ServiceCategoryProperty
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Όνομα Ιδιότητας")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Περιγραφή")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Τύπος Δεδομένων")]
        public string DataType { get; set; } = "string"; // string, number, boolean, etc.

        [Display(Name = "Είναι Υποχρεωτικό")]
        public bool IsRequired { get; set; }

        public int CategoryId { get; set; }
        public virtual ServiceCategory Category { get; set; } = null!;
    }
}
