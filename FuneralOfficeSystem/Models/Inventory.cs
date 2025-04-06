using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuneralOfficeSystem.Models
{
    public class Inventory
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ποσότητα")]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Τιμή Μονάδας")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Συνολική Αξία")]
        public decimal TotalValue { get; set; }

        [StringLength(500)]
        [Display(Name = "Σημειώσεις")]
        public string? Notes { get; set; }

        // Foreign keys
        public int FuneralOfficeId { get; set; }
        public int ProductId { get; set; }

        // Navigation properties
        public virtual FuneralOffice FuneralOffice { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}