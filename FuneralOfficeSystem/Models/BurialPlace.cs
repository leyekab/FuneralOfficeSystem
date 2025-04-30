using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FuneralOfficeSystem.Models
{
    public class BurialPlace
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Το όνομα είναι υποχρεωτικό")]
        [StringLength(50)]
        [Display(Name = "Όνομα")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Ενεργός")]
        [DefaultValue(true)]
        public bool IsEnabled { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Funeral> Funerals { get; set; } = new List<Funeral>();
    }
}
