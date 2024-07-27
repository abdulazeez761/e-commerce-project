//using ECommerceWebsite.Context;
//using ECommerceWebsite.Extensions;
//using ECommerceWebsite.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;

//namespace ECommerceWebsite.Controllers
//{
//    public class OrderItemsController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public OrderItemsController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // GET: OrderItems
//        public async Task<IActionResult> Index()
//        {
//            var applicationDbContext = _context.OrderItems.Include(o => o.Order).Include(o => o.Product);
//            return View(await applicationDbContext.ToListAsync());
//        }

//        // GET: OrderItems/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var orderItem = await _context.OrderItems
//                .Include(o => o.Order)
//                .Include(o => o.Product)
//                .FirstOrDefaultAsync(m => m.OrderItemID == id);
//            if (orderItem == null)
//            {
//                return NotFound();
//            }

//            return View(orderItem);
//        }

//        // GET: OrderItems/Create
//        public IActionResult Create()
//        {
//            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID");
//            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductName");
//            return View();
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("OrderItemID,OrderID,ProductID,Quantity,UnitPrice,Discount")] OrderItem orderItem)
//        {
//            var cart = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("Cart") ?? new List<OrderItem>();
//            var existingItem = cart.FirstOrDefault(i => i.ProductID == orderItem.ProductID);

//            if (existingItem != null)
//            {
//                existingItem.Quantity += orderItem.Quantity;
//            }
//            else
//            {
//                cart.Add(orderItem);
//            }

//            HttpContext.Session.SetObjectAsJson("Cart", cart);
//            return RedirectToAction(nameof(Index), "Products");
//        }

//        // GET: OrderItems/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var orderItem = await _context.OrderItems.FindAsync(id);
//            if (orderItem == null)
//            {
//                return NotFound();
//            }

//            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", orderItem.OrderID);
//            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductName", orderItem.ProductID);
//            return View(orderItem);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("OrderItemID,OrderID,ProductID,Quantity,UnitPrice,Discount")] OrderItem orderItem)
//        {
//            if (id != orderItem.OrderItemID)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(orderItem);
//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!OrderItemExists(orderItem.OrderItemID))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction(nameof(Index));
//            }

//            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", orderItem.OrderID);
//            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductName", orderItem.ProductID);
//            return View(orderItem);
//        }

//        // GET: OrderItems/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var orderItem = await _context.OrderItems
//                .Include(o => o.Order)
//                .Include(o => o.Product)
//                .FirstOrDefaultAsync(m => m.OrderItemID == id);
//            if (orderItem == null)
//            {
//                return NotFound();
//            }

//            return View(orderItem);
//        }

//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var orderItem = await _context.OrderItems.FindAsync(id);
//            if (orderItem != null)
//            {
//                var cart = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("Cart");
//                cart.Remove(orderItem);
//                HttpContext.Session.SetObjectAsJson("Cart", cart);

//                _context.OrderItems.Remove(orderItem);
//                await _context.SaveChangesAsync();
//            }

//            return RedirectToAction(nameof(Index));
//        }

//        private bool OrderItemExists(int id)
//        {
//            return _context.OrderItems.Any(e => e.OrderItemID == id);
//        }

//        // Increase Quantity
//        public IActionResult IncreaseQuantity(int productId)
//        {
//            var cart = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("Cart") ?? new List<OrderItem>();
//            var item = cart.FirstOrDefault(i => i.ProductID == productId);
//            if (item != null)
//            {
//                item.Quantity++;
//                HttpContext.Session.SetObjectAsJson("Cart", cart);
//            }
//            return RedirectToAction(nameof(Index), "Products");
//        }

//        // Decrease Quantity
//        public IActionResult DecreaseQuantity(int productId)
//        {
//            var cart = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("Cart") ?? new List<OrderItem>();
//            var item = cart.FirstOrDefault(i => i.ProductID == productId);
//            if (item != null)
//            {
//                if (item.Quantity > 1)
//                {
//                    item.Quantity--;
//                }
//                else
//                {
//                    cart.Remove(item);
//                }
//                HttpContext.Session.SetObjectAsJson("Cart", cart);
//            }
//            return RedirectToAction(nameof(Index), "Products");
//        }
//    }
//}
