using ECommerceWebsite.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceWebsite.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }

        [Required, MaxLength(100)]
        public string ProductName { get; set; }

        [MaxLength(255)]
        public string ProductDescription { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Discount { get; set; } = 0;

        [EnumDataType(typeof(ProductStatus))]
        public ProductStatus ProductStatus { get; set; } = ProductStatus.Active;
        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public Category Category { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; } = 0;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public ICollection<ProductPhoto> ProductPhotos { get; set; } = new List<ProductPhoto>();


        //public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // we might need it to calculate how many times products is ordered
    }
}
