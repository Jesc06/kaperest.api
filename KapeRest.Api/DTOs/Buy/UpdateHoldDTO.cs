namespace KapeRest.Api.DTOs.Buy
{
    public class UpdateHoldDTO
    {
        public int SalesTransactionID { get; set; }
        public int Quantity { get; set; }
        public decimal DiscountPercent { get; set; } = 0;
        public decimal Tax { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
    }
}
