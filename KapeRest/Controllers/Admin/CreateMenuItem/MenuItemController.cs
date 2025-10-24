using KapeRest.Application.Services.Admin.CreateMenuItem;
using KapeRest.Domain.Entities.MenuEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KapeRest.DTOs.Admin.CreateMenuItem;
using KapeRest.Application.DTOs.Admin.CreateMenuItem;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;


namespace KapeRest.Controllers.Admin.CreateMenuItem
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly MenuItemService _menuItemService;
        public MenuItemController(MenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        [HttpPost("CreateMenuItems")]
        public async Task<ActionResult> CreateMenuItems([FromForm]API_MenuItemDTO dto)
        {
            if (dto == null)
                return BadRequest("The dto field is required.");

            string imagePath = null;

            // ✅ Save image in local folder
            if (dto.image != null)
            {
                var fileName = $"{Guid.NewGuid()}_{dto.image.FileName}";
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fullPath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.image.CopyToAsync(stream);
                }

                imagePath = $"uploads/{fileName}";
            }

            // ✅ Deserialize Products (manual for FromForm)
            var products = new List<ProductQuantityDTO>();
            if (Request.Form.ContainsKey("Products"))
            {
                try
                {
                    var jsonProducts = Request.Form["Products"];
                    products = JsonSerializer.Deserialize<List<ProductQuantityDTO>>(jsonProducts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to deserialize products: {ex.Message}");
                }
            }

            // ✅ Create DTO for Service
            var createDto = new CreateMenuItemDTO
            {
                ItemName = dto.ItemName,
                Price = dto.Price,
                Description = dto.Description,
                image = imagePath,
                Products = products
            };

            var result = await _menuItemService.CreateMenuItem(createDto);

            return Ok(new
            {
                Message = "✅ Menu Item created successfully!",
                MenuItem = result,
                LinkedProducts = products.Count
            });

        }


    }
}
