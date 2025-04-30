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
        [Required(ErrorMessage = "Η διεύθυνση είναι υποχρεωτική")]
        [StringLength(100)]
        [Display(Name = "Διεύθυνση")]
        public string Address { get; set; } = string.Empty;
        [StringLength(20)]
        [Display(Name = "Τηλέφωνο")]
        public string? Phone { get; set; }
        [Display(Name = "Ενεργός")]
        [DefaultValue(true)]
        public bool IsEnabled { get; set; } = true;
        // Navigation properties
        public virtual ICollection<Funeral> Funerals { get; set; } = new List<Funeral>();
    }
}