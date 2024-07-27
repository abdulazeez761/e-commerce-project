namespace ECommerceWebsite.Models
{
    public class ProductIndexViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }

}
