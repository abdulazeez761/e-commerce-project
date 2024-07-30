using ECommerceWebsite.Context;
using ECommerceWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ECommerceWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole(Constants.Roles.Admin))
            {

                return RedirectToAction("Index", "Users");
            }
            else
            {
                // If the condition is not met, fetch the testimonials
                var testemonials = await _context.Testimonials
                                                 .Where(t => t.Status == Constants.TestimonialStatus.Approved)
                                                 .Include(t => t.User)
                                                 .ToListAsync();
                if (testemonials == null || !testemonials.Any())
                {
                    return View(new List<Testimonial>());
                }

                return View(testemonials);
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
