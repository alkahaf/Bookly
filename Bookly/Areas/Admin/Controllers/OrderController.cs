﻿using Bookly.DataAccess.Repository.IRepository;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bookly.Areas.Admin.Controllers
{
    [Area("admin")]
    public class OrderController : Controller

    {

        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {

            return View();
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<OrderHeader> objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            return Json(new { data = objOrderHeaders });
        }





        #endregion
    }
}
