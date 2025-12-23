using KapeRest.Application.DTOs.Admin.CreateMenuItem;
using Microsoft.AspNetCore.Http;

namespace KapeRest.Api.DTOs.MenuItem
{
    public class CreateMenuItemDTOs
    {
        public string Item_name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public IFormFile? Image { get; set; }
        public string IsAvailable { get; set; } 
        public string ProductsJson { get; set; }
    }
}
