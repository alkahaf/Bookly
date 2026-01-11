using System.IO;
using System.Text.Json;
using Bookly.DataAccess.Repository.IRepository;
using Bookly.Utility;
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

        public WebhookController(IOptions<RazorpaySettings> razorpayOptions,
                                 IUnitOfWork unitOfWork)
        {
            _razorpay = razorpayOptions.Value;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Receive()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var signature = Request.Headers["X-Razorpay-Signature"];

            try
            {
                Utils.verifyWebhookSignature(body, signature, _razorpay.Secret);
            }
            catch
            {
                return BadRequest();
            }

            var json = JsonDocument.Parse(body);
            var paymentId = json.RootElement
                .GetProperty("payload")
                .GetProperty("payment")
                .GetProperty("entity")
                .GetProperty("id")
                .GetString();

            var orderHeader =
                _unitOfWork.OrderHeader.Get(o => o.PaymentIntentId == paymentId);

            if (orderHeader != null)
            {
                orderHeader.PaymentStatus = SD.PaymentStatusApproved;
                orderHeader.OrderStatus = SD.StatusApproved;
                _unitOfWork.OrderHeader.Update(orderHeader);
                _unitOfWork.Save();
            }

            return Ok();
        }
    }
}
