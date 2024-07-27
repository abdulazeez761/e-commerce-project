//using ECommerceWebsite.Constants;
//using ECommerceWebsite.Context;
//using ECommerceWebsite.Extensions;
//using ECommerceWebsite.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;

//namespace ECommerceWebsite.Controllers
//{
//    public class OrdersController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public OrdersController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // GET: Orders
//        public async Task<IActionResult> Index()
//        {
//            var applicationDbContext = _context.Orders.Include(o => o.User);
//            return View(await applicationDbContext.ToListAsync());
//        }

//        // GET: Orders/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var order = await _context.Orders
//                                      .Include(o => o.OrderItems)
//                                      .ThenInclude(oi => oi.Product)
//                                      .FirstOrDefaultAsync(m => m.OrderID == id);

//            if (order == null)
//            {
//                return NotFound();
//            }

//            return View(order);
//        }

//        // GET: Orders/Create
//        public IActionResult Create()
//        {
//            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Email");
//            return View();
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("OrderID,UserID,Address,OrderDate,TotalAmount,OrderStatus")] Order order)
//        {
//            if (ModelState.IsValid)
//            {
//                User user = await _context.Users.FindAsync(order.UserID);
//                user.Orders.Add(order);
//                order.User = user;

//                try
//                {
//                    _context.Add(order);
//                    await _context.SaveChangesAsync();
//                    return RedirectToAction(nameof(Index));
//                }
//                catch (DbUpdateException ex)
//                {
//                    Console.WriteLine(ex.Message);
//                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
//                }
//            }

//            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Email", order.UserID);
//            return View(order);
//        }

//        // Add SetOrderStatus function
//        public async Task SetOrderStatus(int id, OrderStatus orderStatus)
//        {
//            if (OrderExists(id))
//            {
//                var order = await _context.Orders.FindAsync(id);
//                if (order != null)
//                {
//                    order.OrderStatus = orderStatus;
//                    await _context.SaveChangesAsync();
//                }
//            }
//        }

//        // Checkout function
//        public async Task<IActionResult> Checkout()
//        {
//            // Retrieve cart data from session
//            var cart = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("Cart");

//            if (cart == null || !cart.Any())
//            {
//                ModelState.AddModelError("", "Cart is empty. Please add items to the cart before checking out.");
//                return RedirectToAction(nameof(Index), "Products");
//            }

//            var order = new Order
//            {
//                UserID = 1, // Placeholder, should use authenticated user ID
//                OrderDate = DateTime.Now,
//                TotalAmount = cart.Sum(item => item.Quantity * item.UnitPrice),
//                OrderStatus = OrderStatus.Pending,
//                OrderItems = cart
//            };

//            _context.Orders.Add(order);
//            await _context.SaveChangesAsync();

//            // Clear the session cart
//            HttpContext.Session.Remove("Cart");

//            return RedirectToAction(nameof(Index));
//        }

//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var order = await _context.Orders
//                .Include(o => o.User)
//                .FirstOrDefaultAsync(m => m.OrderID == id);

//            if (order == null)
//            {
//                return NotFound();
//            }

//            return View(order);
//        }

//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(m => m.OrderID == id);
//            if (order != null)
//            {
//                _context.Orders.Remove(order);
//            }

//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool OrderExists(int id)
//        {
//            return _context.Orders.Any(e => e.OrderID == id);
//        }
//    }
//}
