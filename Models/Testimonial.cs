using ECommerceWebsite.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ECommerceWebsite.Models
{
    public class Testimonial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TestimonialID { get; set; }

        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        [Required, MaxLength(1000)]
        public string Content { get; set; }

        [Required]
        [EnumDataType(typeof(TestimonialStatus))]
        public TestimonialStatus Status { get; set; } = TestimonialStatus.Pending;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    }
}
