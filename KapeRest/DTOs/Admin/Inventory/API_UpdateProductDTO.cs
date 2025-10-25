namespace KapeRest.DTOs.Admin.Inventory
{
    public class API_UpdateProductDTO
    {
        public int Id { get; set; } 
        public string ProductName { get; set; }
        public decimal Prices { get; set; }
        public int Stocks { get; set; }
        public string Units { get; set; }

    }
}
