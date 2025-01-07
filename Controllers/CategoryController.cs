using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bookly.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db) {
        _db = db;   
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _db.Category.ToList();
            return View(objCategoryList);
        }
    }
}
