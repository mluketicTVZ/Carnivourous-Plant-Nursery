using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carnivorous_Plant_Nursery.Models
{
    public abstract class Article
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? SKU { get; set; }

        [MaxLength(200)]
        public string? ListingTitle { get; set; }

        private decimal? _price;
        [Range(typeof(decimal), "0", "999999.99")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price
        {
            get => _price;
            set
            {
                if (value.HasValue && value.Value < 0m)
                    throw new ArgumentOutOfRangeException(nameof(value), ErrorMessage.NegativePrice);
                _price = value;
            }
        }

        public bool IsAvailableInWebshop { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime? DeletedAt { get; set; }

        protected Article() {}
    }
}
