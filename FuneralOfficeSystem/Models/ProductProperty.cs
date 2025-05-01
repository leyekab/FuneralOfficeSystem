using System.ComponentModel.DataAnnotations;

namespace FuneralOfficeSystem.Models
{
    public class ProductProperty
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        public int CategoryPropertyId { get; set; }
        public virtual CategoryProperty CategoryProperty { get; set; } = null!;

        [Required]
        [Display(Name = "Τιμή")]
        public string Value { get; set; } = string.Empty;
    }
}