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
                var order = new Order
                {
                    UserID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    OrderDate = DateTime.Now,
                    TotalAmount = cart.Sum(item => item.Quantity * item.UnitPrice),
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


    }
}
