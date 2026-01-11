using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Bookly.DataAccess.Repository.IRepository;
using Bookly.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Razorpay.Api;

namespace Bookly.Controllers
{
    [ApiController]
    [Route("api/webhook/razorpay")]
    public class WebhookController : ControllerBase
    {
        private readonly RazorpaySettings _razorpay;
        private readonly IUnitOfWork _unitOfWork;

        public WebhookController(IOptions<RazorpaySettings> razorpayOptions, IUnitOfWork unitOfWork)
        {
            _razorpay = razorpayOptions.Value;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Receive()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var signature = Request.Headers["X-Razorpay-Signature"].FirstOrDefault();

            if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(_razorpay.Secret))
                return BadRequest();

            try
            {
                // Throws if invalid
                Utils.verifyWebhookSignature(body, signature, _razorpay.Secret);
            }
            catch
            {
                return BadRequest();
            }

            var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            var eventType = root.GetProperty("event").GetString();

            // handle event types you need (example: payment.captured, order.paid)
            if (eventType == "payment.captured" || eventType == "order.paid")
            {
                // attempt to extract payment id and order id from payload
                if (root.TryGetProperty("payload", out var payload) &&
                    payload.TryGetProperty("payment", out var paymentWrapper) &&
                    paymentWrapper.TryGetProperty("entity", out var paymentEntity))
                {
                    var paymentId = paymentEntity.GetProperty("id").GetString();
                    var orderId = paymentEntity.TryGetProperty("order_id", out var o) ? o.GetString() : null;

                    // find order header by PaymentIntentId or by previously stored razorpay order id if you saved it
                    var orderHeader = _unitOfWork.OrderHeader.Get(h => h.PaymentIntentId == paymentId 
                                                                       || h.PaymentIntentId == orderId);
                    if (orderHeader != null)
                    {
                        orderHeader.PaymentStatus = SD.PaymentStatusApproved;
                        orderHeader.OrderStatus = SD.StatusApproved;
                        _unitOfWork.OrderHeader.Update(orderHeader);
                        _unitOfWork.Save();
                    }
                }
            }

            return Ok();
        }
    }
}