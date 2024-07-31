using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebsite.Controllers
{
    public class DashBoardeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
