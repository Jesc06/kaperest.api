namespace KapeRest.DTOs.Admin.Inventory
{
    public class API_CreateProductDTO
    {
        public string ProductName { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int SupplierId { get; set; }

    }
}
