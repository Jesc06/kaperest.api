namespace KapeRest.Application.DTOs.PayMongo
{
    public class PaymentVerificationResult
    {
        public string Status { get; set; } = string.Empty;
        public string PaymentReference { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
