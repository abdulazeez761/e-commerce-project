using ECommerceWebsite.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceWebsite.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }


        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        [MaxLength(255)]
        public string Address { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required, Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }


        [EnumDataType(typeof(OrderStatus))]
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public string FullName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
