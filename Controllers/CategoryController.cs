using Microsoft.AspNetCore.Mvc;

namespace Bookly.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
