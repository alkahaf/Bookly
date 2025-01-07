﻿using Bookly.Data;
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
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            //if(obj.Name == obj.DisplayOrder.ToString()) 
            //{
            //    ModelState.AddModelError("name","The DisplayOrder cannot exactly match the Name.");
            //}
            
            if (ModelState.IsValid)
            {
                _db.Category.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
