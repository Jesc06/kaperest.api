namespace KapeRest.DTOs.Admin.Inventory
{
    public class API_CreateProductDTO
    {
        public string ProductName { get; set; }
        public decimal CostPrice { get; set; }
        public int Stock { get; set; }
        public string Units { get; set; }
        public int SupplierId { get; set; }

    }
}
