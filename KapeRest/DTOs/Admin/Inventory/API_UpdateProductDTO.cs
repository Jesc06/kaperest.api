namespace KapeRest.DTOs.Admin.Inventory
{
    public class API_UpdateProductDTO
    {
        public int Id { get; set; } 
        public string ProductName { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; }

        //Image file upload
        public IFormFile ImageFile { get; set; }
    }
}
