using System.ComponentModel.DataAnnotations;

namespace FuneralOfficeSystem.Models
{
    public class CategoryProperty
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Το όνομα της ιδιότητας είναι υποχρεωτικό")]
        [StringLength(100)]
        [Display(Name = "Όνομα Ιδιότητας")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Περιγραφή")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Τύπος Δεδομένων")]
        public string DataType { get; set; } = "string"; // string, number, boolean, etc.

        [Display(Name = "Είναι Υποχρεωτικό")]
        public bool IsRequired { get; set; }

        public int CategoryId { get; set; }
        public virtual ProductCategory Category { get; set; } = null!;
    }
}