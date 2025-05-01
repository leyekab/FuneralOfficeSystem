using System.ComponentModel.DataAnnotations;

namespace FuneralOfficeSystem.Models
{
    // Τιμές ιδιοτήτων υπηρεσίας
    public class ServiceProperty
    {
        public int Id { get; set; }

        public int ServiceId { get; set; }
        public virtual Service Service { get; set; } = null!;

        public int CategoryPropertyId { get; set; }
        public virtual ServiceCategoryProperty CategoryProperty { get; set; } = null!;

        [Required]
        [Display(Name = "Τιμή")]
        public string Value { get; set; } = string.Empty;
    }
}
