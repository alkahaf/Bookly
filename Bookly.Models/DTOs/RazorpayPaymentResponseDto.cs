namespace Bookly.Models.DTOs
{
    public class RazorpayPaymentResponseDto
    {
        public string razorpay_payment_id { get; set; }
        public string razorpay_order_id { get; set; }
        public string razorpay_signature { get; set; }

        public int orderHeaderId { get; set; }

    }
}