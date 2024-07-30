using ECommerceWebsite.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceWebsite.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [Required, MaxLength(100)]
        public string UserName { get; set; }

        [Required, MaxLength(100), EmailAddress]
        public string Email { get; set; }


        public string UserPhoto { get; set; }

        [Required]
        public string PasswordHash { get; set; }
        public string Salt { get; set; }

        [MaxLength(100)]
        [Required]
        public string FirstName { get; set; }

        [MaxLength(100)]
        [Required]

        public string LastName { get; set; }

        [MaxLength(255)]
        [Required]

        public string Address { get; set; }

        [MaxLength(100)]
        [Required]

        public string City { get; set; }

        [MaxLength(100)]
        [Required]

        public string Country { get; set; }

        [MaxLength(20)]
        [Required]

        public string PostalCode { get; set; }

        [MaxLength(20), Phone]
        [Required]

        public string PhoneNumber { get; set; }

        public string UserType { get; set; } = Roles.User;

        [EnumDataType(typeof(AccountStatus))]
        public AccountStatus AccountStatus { get; set; } = AccountStatus.Active;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Testimonial> Testimonials { get; set; } = new List<Testimonial>();
    }
}
