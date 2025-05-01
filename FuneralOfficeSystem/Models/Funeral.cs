using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuneralOfficeSystem.Models
{
    public class Funeral
    {
        public int Id { get; set; }

        //[Required(ErrorMessage = "Ο τόπος ταφής είναι υποχρεωτικός")]
        //[StringLength(100)]
        //[Display(Name = "Τόπος Ταφής")]
        //public string BurialPlace { get; set; } = string.Empty;

        //[Required(ErrorMessage = "Η εκκλησία είναι υποχρεωτική")]
        //[StringLength(100)]
        //[Display(Name = "Εκκλησία")]
        //public string Church { get; set; } = string.Empty;

        [Required(ErrorMessage = "Η ώρα τελετής είναι υποχρεωτική")]
        [StringLength(50)]
        [Display(Name = "Ώρα Τελετής")]
        public string CeremonyTime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Η ημερομηνία τελετής είναι υποχρεωτική")]
        [DataType(DataType.Date)]
        [Display(Name = "Ημερομηνία Τελετής")]
        public DateTime FuneralDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Συνολικό Κόστος")]
        public decimal TotalCost { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Προκαταβολή")]
        public decimal Advance { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Υπόλοιπο")]
        public decimal Balance { get; set; }

        [Display(Name = "Τελικός Λογαριασμός")]
        public bool IsFinalBill { get; set; } = false;

        [StringLength(500)]
        [Display(Name = "Σημειώσεις")]
        public string? Notes { get; set; }

        // Foreign keys
        public int FuneralOfficeId { get; set; }
        public int DeceasedId { get; set; }
        public int ClientId { get; set; }
        public int ChurchId { get; set; }
        public int BurialPlaceId { get; set; }

        // Navigation properties
        public virtual FuneralOffice FuneralOffice { get; set; } = null!;
        public virtual Deceased Deceased { get; set; } = null!;
        public virtual Client Client { get; set; } = null!;
        public virtual Church Church { get; set; } = null!;
        public virtual BurialPlace BurialPlace { get; set; } = null!;
        public virtual ICollection<FuneralProduct> FuneralProducts { get; set; } = new List<FuneralProduct>();
        public virtual ICollection<FuneralService> FuneralServices { get; set; } = new List<FuneralService>();
    }
}