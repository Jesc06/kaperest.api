namespace KapeRest.Api.DTOs.AddProducts
{
    public class InventoryDTO
    {
        public string ProductName { get; set; }
        public decimal CostPrice { get; set; }
        public int Stocks { get; set; }
        public string Units { get; set; }

        //connect to Supplier
        public int SupplierId { get; set; }
    }
}
