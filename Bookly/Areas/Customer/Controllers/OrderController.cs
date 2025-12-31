using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;

namespace Bookly.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        [HttpPost]
        public IActionResult CreateRazorPayOrder([FromBody] decimal orderTotalUsd)
        {
            // 💱 USD → INR
            decimal usdToInrRate = 83;
            decimal orderTotalInr = orderTotalUsd * usdToInrRate;

            // Razorpay expects amount in paise
            int amountInPaise = (int)(orderTotalInr * 100);

            var client = new RazorpayClient(
                "rzp_test_RyITi31LLPTGXo",
                "qH5cQqhcvoTDf2Wb5NSmklhf"
            );

            var options = new Dictionary<string, object>
            {
                { "amount", amountInPaise },
                { "currency", "INR" },
                { "receipt", Guid.NewGuid().ToString() }
            };

            var order = client.Order.Create(options);

            return Json(new
            {
                key = "rzp_test_RyITi31LLPTGXo",
                orderId = order["id"],
                amount = amountInPaise
            });
        }

        // 🔐 Payment Success Handler
        [HttpPost]
        public IActionResult PaymentSuccess(
            string razorpay_payment_id,
            string razorpay_order_id,
            string razorpay_signature)
        {
            // ✅ Here you will:
            // 1. Verify signature (later)
            // 2. Save order to DB
            // 3. Reduce stock
            // 4. Clear cart

            return Ok(new { success = true });
        }
    }
}
