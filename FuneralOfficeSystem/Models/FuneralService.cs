using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuneralOfficeSystem.Models
{
    public class FuneralService
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Τιμή")]
        public decimal Price { get; set; }

        [StringLength(500)]
        [Display(Name = "Σημειώσεις")]
        public string? Notes { get; set; }

        // Foreign keys
        public int FuneralId { get; set; }
        public int ServiceId { get; set; }

        // Navigation properties
        public virtual Funeral Funeral { get; set; } = null!;
        public virtual Service Service { get; set; } = null!;
    }
}