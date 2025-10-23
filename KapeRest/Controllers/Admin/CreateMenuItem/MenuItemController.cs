using KapeRest.Application.Services.Admin.CreateMenuItem;
using KapeRest.Domain.Entities.MenuEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KapeRest.DTOs.Admin.CreateMenuItem;
using KapeRest.Application.DTOs.Admin.CreateMenuItem;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;


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

            byte[] imageBytes = null;
            if (dto.image != null)
            {
                using (var ms = new MemoryStream())
                {
                    await dto.image.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }
            }

            var createDto = new CreateMenuItemDTO
            {
                ItemName = dto.ItemName,
                Price = dto.Price,
                Description = dto.Description,
                image = imageBytes,
                Products = dto.Products
            };

            var result = await _menuItemService.CreateMenuItem(createDto);
            return Ok(result);

        }


    }
}
