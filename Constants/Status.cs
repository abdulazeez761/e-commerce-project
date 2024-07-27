namespace ECommerceWebsite.Constants
{
    public enum TestimonialStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public enum AccountStatus
    {
        Active,
        Inactive,
    }
    public static class Roles
    {
        public const string User = "User";
        public const string Admin = "Admin";
    }

    public enum ProductStatus
    {
        Active,
        OutOfStock,
        Deleted
    }

    public enum CategoryStatus
    {
        Active,
        Inactive
    }

    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

}
