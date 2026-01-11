
namespace Bookly.Models.ViewModels
{
    public class OrderConfirmationVM
    {
        public int OrderHeaderId { get; set; }
        public string RazorpayKey { get; set; }
        public string RazorpayOrderId { get; set; }
        public int AmountInPaise { get; set; }
    }
}