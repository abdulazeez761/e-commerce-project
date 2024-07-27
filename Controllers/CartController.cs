using ECommerceWebsite.Constants;
using ECommerceWebsite.Context;
using ECommerceWebsite.Extensions;
using ECommerceWebsite.Models;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Checkout()
        {
            /* logic:
             *connecting the order items with order and connecting the order with the user
             *calculating the total amount of the order including discounts
             *
             */
            var cart = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("Cart") ?? new List<OrderItem>();

            if (!cart.Any())
            {
                ModelState.AddModelError("", "Cart is empty. Please add items to the cart before checking out.");
                return RedirectToAction(nameof(Index));
            }

            var order = new Order
            {
                UserID = 1, // Placeholder for authenticated user ID
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(item => item.Quantity * item.UnitPrice),
                OrderStatus = OrderStatus.Pending,
                OrderItems = cart
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Index", "Orders");
        }
    }
}
