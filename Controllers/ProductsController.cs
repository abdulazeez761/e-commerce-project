using ECommerceWebsite.Context;
using ECommerceWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Products/Details/5
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

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, List<IFormFile> photos)
        {
            Category category = await _context.Categories.FindAsync(product.CategoryID);

            if (category == null || category.CategoryStatus == Constants.CategoryStatus.Inactive)
                throw new Exception("Category not found");

            product.Category = category;
            category.Products.Add(product);




            //adding  productImages

            var webRootPath = Path.Combine(_hostEnvironment.WebRootPath, "images/productsImages");
            if (!Directory.Exists(webRootPath))
            {
                Directory.CreateDirectory(webRootPath);
            }

            //adding main product image
            var mainProductImage = photos[0];
            Guid mainImage = Guid.NewGuid();
            string fullPathForMain = Path.Combine(webRootPath, mainImage + Path.GetExtension(mainProductImage.FileName));

            using (var fileStream = new FileStream(fullPathForMain, FileMode.Create))
            {
                mainProductImage.CopyTo(fileStream);
            }
            product.ProductImage = Path.GetFileName(fullPathForMain);

            //adding the rest of the product images
            foreach (var photo in photos)
            {
                Guid picName = Guid.NewGuid();
                string fullPath = Path.Combine(webRootPath, picName + Path.GetExtension(photo.FileName));
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    photo.CopyTo(fileStream);
                }
                var productPhoto = new ProductPhoto
                {
                    ProductID = product.ProductID,
                    FilePath = Path.GetFileName(fullPath)
                };
                _context.ProductPhotos.Add(productPhoto);

                product.ProductPhotos.Add(productPhoto);
            }
            try
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,ProductName,ProductDescription,Price,Discount,ProductStatus,CategoryID,Stock,DateCreated")] Product product)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }



        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
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
        public async Task<IActionResult> SoftDelete(int id)
        {

            if (!ProductExists(id))
                return NotFound();

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductID == id);
            product.ProductStatus = Constants.ProductStatus.Deleted;

            return RedirectToAction(nameof(Index));
        }
        public async Task SetProductToOutOfStuck(int id)
        {

            if (ProductExists(id))
            {
                var product = await _context.Products
            .FirstOrDefaultAsync(m => m.ProductID == id);
                product.ProductStatus = Constants.ProductStatus.OutOfStock;
            }
        }

        public async Task<IActionResult> ActiveProduct(int id)
        {

            if (!ProductExists(id))
                return NotFound();

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductID == id);
            product.ProductStatus = Constants.ProductStatus.Active;

            return RedirectToAction(nameof(Index));
        }

        //this code is to delete the product from the data base which we dont want to do we only want to soft dlete it whichc means to change teh status.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products
           .Include(p => p.Category)
           .Include(p => p.ProductPhotos)
           .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }
    }
}
