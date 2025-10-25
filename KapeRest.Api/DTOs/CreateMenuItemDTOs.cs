using KapeRest.Application.DTOs.Admin.CreateMenuItem;

namespace KapeRest.Api.DTOs
{
    public class CreateMenuItemDTOs
    {
        public string Item_name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public IFormFile? Image { get; set; }
        public string ProductsJson { get; set; }




    }
}
