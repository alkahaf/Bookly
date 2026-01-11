using Bookly.DataAccess.Repository.IRepository;
using Bookly.Models;
using Bookly.Models.DTOs;
using Bookly.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Razorpay.Api;
using System.Security.Claims;

namespace Bookly.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly RazorpaySettings _razorpay;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentController(
            IOptions<RazorpaySettings> razorpayOptions,
            IUnitOfWork unitOfWork)
        {
            _razorpay = razorpayOptions.Value;
            _unitOfWork = unitOfWork;
        }

        // 🔹 CREATE RAZORPAY ORDER + DB ORDER
        [HttpPost]
        public IActionResult CreateRazorPayOrder([FromBody] OrderCreateDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            OrderHeader orderHeader = new OrderHeader
            {
                ApplicationUserId = userId,
                OrderDate = DateTime.Now,

                // 🔥 STORE ALL DETAILS
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                StreetAddress = model.StreetAddress,
                City = model.City,
                State = model.State,
                PostalCode = model.PostalCode,

                OrderTotal = model.OrderTotal,
                PaymentStatus = SD.PaymentStatusPending,
                OrderStatus = SD.StatusPending
            };

            _unitOfWork.OrderHeader.Add(orderHeader);
            _unitOfWork.Save();

            RazorpayClient client =
                new RazorpayClient(_razorpay.KeyId, _razorpay.Secret);

            var options = new Dictionary<string, object>
    {
        { "amount", Convert.ToInt32(model.OrderTotal * 100) },
        { "currency", "INR" },
        { "receipt", orderHeader.Id.ToString() }
    };

            Order razorpayOrder = client.Order.Create(options);

            return Json(new
            {
                key = _razorpay.KeyId,
                razorpayOrderId = razorpayOrder["id"].ToString(),
                amount = razorpayOrder["amount"],
                orderId = orderHeader.Id
            });
        }

        // 🔹 PAYMENT SUCCESS
        [HttpPost]
        public IActionResult PaymentSuccess([FromBody] RazorpayPaymentResponseDto model)
        {
            var orderHeader = _unitOfWork.OrderHeader
                .Get(o => o.Id == model.orderHeaderId);

            if (orderHeader == null)
            {
                return Json(new { success = false, message = "Order not found" });
            }

            orderHeader.PaymentIntentId = model.razorpay_payment_id;
            orderHeader.PaymentStatus = SD.PaymentStatusApproved;
            orderHeader.OrderStatus = SD.StatusApproved;
            orderHeader.PaymentDate = DateTime.Now;

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();

            return Json(new { success = true });
        }
    }

        // 🔹 Razorpay Response Model
        public class RazorpayResponse
    {
        public string razorpay_payment_id { get; set; }
        public string razorpay_order_id { get; set; }
        public string razorpay_signature { get; set; }

        // 👇 ADD THIS (receipt sent back from JS)
        public string receipt { get; set; }
    }
}
