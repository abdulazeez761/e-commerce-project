using ECommerceWebsite.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceWebsite.Models
{
    public class Code
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CodeID { get; set; }

        [Required]
        [MaxLength(100)]
        public string CodeName { get; set; }

        [Required]
        public decimal DiscountAmount { get; set; }

        [Required]
        public DateTime ExpireDate { get; set; }

        [EnumDataType(typeof(CodeStatus))]
        public CodeStatus IsActive { get; set; } = CodeStatus.Active;

        [MaxLength(500)]
        public string Description { get; set; }
    }
}
