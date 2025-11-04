namespace KapeRest.Api.DTOs.Buy
{
    public class BuyDTO
    {
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
        public decimal DiscountPercent { get; set; } = 0;
        public decimal Tax { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
    }
}
