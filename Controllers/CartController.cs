using ECommerceWebsite.Constants;
using ECommerceWebsite.Context;
using ECommerceWebsite.Extensions;
using ECommerceWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using mvc_first_task.ActionFilters;
using System.Security.Claims;

namespace ECommerceWebsite.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("Cart") ?? new List<OrderItem>();

            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();


            var cart = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("Cart") ?? new List<OrderItem>();

            var existingItem = cart.FirstOrDefault(i => i.ProductID == id);

            if (existingItem != null)
                existingItem.Quantity++;
            else
            {
                cart.Add(new OrderItem
                {
                    OrderItemID = Guid.NewGuid(),
                    ProductID = id,
                    Product = product,
                    Quantity = 1,
                    UnitPrice = product.Price
                });
            }
            HttpContext.Session.SetString("CartCounter", cart.Count().ToString());
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction("Index", "Products");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(Guid orderItemID, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("Cart") ?? new List<OrderItem>();
            var item = cart.FirstOrDefault(i => i.OrderItemID == orderItemID);

            if (item != null)
            {
                item.Quantity = quantity;
                HttpContext.Session.SetObjectAsJson("Cart", cart);
                HttpContext.Session.SetString("CartCounter", cart.Count().ToString());
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult RemoveFromCart(Guid orderItemID)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("Cart") ?? new List<OrderItem>();
            var item = cart.FirstOrDefault(i => i.OrderItemID == orderItemID);

            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
                HttpContext.Session.SetString("CartCounter", cart.Count().ToString());
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [RoleValidation(Constants.Roles.User)]
        public async Task<IActionResult> Checkout(
      string fullName,
      string address,
      string city,
      string state,
      string postalCode,
      string country)
        {
            try
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("Cart") ?? new List<OrderItem>();

                if (!cart.Any())
                {
                    ModelState.AddModelError("", "Cart is empty. Please add items to the cart before checking out.");
                    return RedirectToAction(nameof(Index));
                }

                // Create an Order and populate with billing information
                var discountedChart = HttpContext.Session.GetString("NewTotalAmount");

                decimal totalAmount = 0;
                if (discountedChart is not null) totalAmount = decimal.Parse(discountedChart);
                else totalAmount = cart.Sum(item => item.Quantity * item.UnitPrice);

                var order = new Order
                {
                    UserID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    OrderDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    OrderStatus = OrderStatus.Pending,

                    FullName = fullName,
                    Address = address,
                    City = city,
                    State = state,
                    PostalCode = postalCode,
                    Country = country
                };
                foreach (var item in cart)
                {
                    var orderItem = new OrderItem
                    {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice

                    };

                    order.OrderItems.Add(orderItem);
                }
                var user = _context.Users.FirstOrDefault(x => x.UserID == order.UserID);
                if (user != null)
                {
                    if (user.Orders == null)
                        user.Orders = new List<Order>();

                    user.Orders.Add(order);
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                HttpContext.Session.Remove("Cart");
                HttpContext.Session.Remove("NewTotalAmount");
                HttpContext.Session.SetString("CartCounter", "0");

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Exception occurred: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while processing your request. Please try again.");
                return View("Index", "Home");
            }
        }


        [HttpPost]
        public IActionResult ApplyDiscount(string discountCode)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("Cart") ?? new List<OrderItem>();

            if (string.IsNullOrEmpty(discountCode))
            {
                TempData["ErrorMessage"] = "Discount code is required.";
                return RedirectToAction(nameof(Index));
            }

            var lowerDiscountCode = discountCode.ToLower();
            var discount = _context.Code
                .Where(d => d.CodeName.ToLower() == lowerDiscountCode && d.IsActive == Constants.CodeStatus.Active && d.ExpireDate >= DateTime.Now)
                .FirstOrDefault();

            if (discount == null)
            {
                TempData["ErrorMessage"] = "Invalid or expired discount code.";
                return RedirectToAction(nameof(Index));
            }

            decimal totalAmount = cart.Sum(item => item.Quantity * item.UnitPrice);

            decimal discountAmount = discount.DiscountAmount;
            decimal newTotalAmount = totalAmount - discountAmount;
            var discountedChart = HttpContext.Session.GetString("NewTotalAmount");
            if (discountedChart is not null)
            {
                TempData["ErrorMessage"] = "You already have applied this code";
                return RedirectToAction(nameof(Index));
            }
            HttpContext.Session.SetString("NewTotalAmount", newTotalAmount.ToString());
            TempData["SuccessMessage"] = "Discount code applied successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
