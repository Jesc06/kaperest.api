using KapeRest.Application.DTOs.Admin.CreateMenuItem;

namespace KapeRest.DTOs.Admin.CreateMenuItem
{
    public class API_MenuItemDTO
    {
        public string ItemName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; }
        public IFormFile image { get; set; }

        public List<ProductQuantityDTO>? Products { get; set; } = new();
    }
}
