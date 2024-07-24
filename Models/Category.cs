using ECommerceWebsite.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceWebsite.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryID { get; set; }

        [Required, MaxLength(100)]
        public string CategoryName { get; set; }

        [MaxLength(255)]
        public string CategoryDescription { get; set; }

        [Required]
        [EnumDataType(typeof(CategoryStatus))]
        public CategoryStatus CategoryStatus { get; set; } = CategoryStatus.Active;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public ICollection<Product> Products { get; set; }

    }
}
