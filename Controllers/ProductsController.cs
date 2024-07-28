using ECommerceWebsite.Context;
using ECommerceWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceWebsite.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;


        public ProductsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductPhotos)
                //.Take(3)
                .ToListAsync();

            var categories = await _context.Categories.ToListAsync();

            var viewModel = new ProductIndexViewModel
            {
                Products = products,
                Categories = categories
            };

            return View(viewModel);
        }
        public async Task<IActionResult> AdminProductIndex()
        {
            /* var products = await _context.Products
                 .Include(p => p.Category)
                 .Include(p => p.ProductPhotos)
                 //.Take(3)
                 .ToListAsync();

             var categories = await _context.Categories.ToListAsync();

             var viewModel = new ProductIndexViewModel
             {
                 Products = products,
                 Categories = categories
             };*/

            return View();
        }
        public async Task<IActionResult> LoadMoreProducts(int skip, int take)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductPhotos)
                .OrderBy(p => p.ProductID)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return PartialView("_ProductListPartial", products);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductPhotos)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }











        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }
    }
}
