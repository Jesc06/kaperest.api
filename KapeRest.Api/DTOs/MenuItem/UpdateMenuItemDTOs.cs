using Microsoft.AspNetCore.Http;

namespace KapeRest.Api.DTOs.MenuItem
{
    public class UpdateMenuItemDTOs
    {
        public int Id { get; set; }
        public string cashierId { get; set; }
        public string Item_name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }    
        public string Description { get; set; }
        public IFormFile? Image { get; set; }
        public string IsAvailable { get; set; }
        public string ProductsJson { get; set; }
    }
}
