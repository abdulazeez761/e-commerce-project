using ECommerceWebsite.Context;
using ECommerceWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceWebsite.Controllers
{
    public class CodesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CodesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Codes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Code.ToListAsync());
        }

        // GET: Codes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var code = await _context.Code
                .FirstOrDefaultAsync(m => m.CodeID == id);
            if (code == null)
            {
                return NotFound();
            }

            return View(code);
        }

        // GET: Codes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Codes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodeID,CodeName,DiscountAmount,ExpireDate,IsActive,Description")] Code code)
        {

            _context.Code.Add(code);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }

        // GET: Codes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var code = await _context.Code.FindAsync(id);
            if (code == null)
            {
                return NotFound();
            }
            return View(code);
        }

        // POST: Codes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodeID,CodeName,DiscountAmount,ExpireDate,IsActive,Description")] Code code)
        {
            if (id != code.CodeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(code);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CodeExists(code.CodeID))
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
            return View(code);
        }

        // GET: Codes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var code = await _context.Code
                .FirstOrDefaultAsync(m => m.CodeID == id);
            if (code == null)
            {
                return NotFound();
            }

            return View(code);
        }

        // POST: Codes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var code = await _context.Code.FindAsync(id);
            if (code != null)
            {
                _context.Code.Remove(code);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CodeExists(int id)
        {
            return _context.Code.Any(e => e.CodeID == id);
        }
    }
}
