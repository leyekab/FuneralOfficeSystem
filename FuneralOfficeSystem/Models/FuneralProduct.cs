using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuneralOfficeSystem.Models
{
    public class FuneralProduct
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ποσότητα")]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Τιμή Μονάδας")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Τιμή")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Συνολική Τιμή")]
        public decimal TotalPrice { get; set; }

        [StringLength(500)]
        [Display(Name = "Σημειώσεις")]
        public string? Notes { get; set; }

        // Foreign keys
        public int FuneralId { get; set; }
        public int ProductId { get; set; }

        // Navigation properties
        public virtual Funeral Funeral { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}