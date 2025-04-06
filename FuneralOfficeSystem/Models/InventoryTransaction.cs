using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuneralOfficeSystem.Models
{
    public enum TransactionTypeEnum
    {
        [Display(Name = "Προσθήκη")]
        Addition,
        [Display(Name = "Αφαίρεση")]
        Subtraction,
        [Display(Name = "Μεταφορά")]
        Transfer
    }

    public class InventoryTransaction
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Τύπος Συναλλαγής")]
        public TransactionTypeEnum TransactionTypeEnum { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Τύπος Συναλλαγής")]
        public string TransactionType { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Ποσότητα")]
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Ημερομηνία Συναλλαγής")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [StringLength(500)]
        [Display(Name = "Σημειώσεις")]
        public string? Notes { get; set; }

        // Foreign keys
        public int ProductId { get; set; }
        public int SourceFuneralOfficeId { get; set; }
        public int? DestinationFuneralOfficeId { get; set; }

        // Navigation properties
        public virtual Product Product { get; set; } = null!;
        public virtual FuneralOffice SourceFuneralOffice { get; set; } = null!;
        public virtual FuneralOffice? DestinationFuneralOffice { get; set; }
    }
}