using KapeRest.Application.Services.Admin.CreateMenuItem;
using KapeRest.Domain.Entities.MenuEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KapeRest.Application.DTOs.Admin.CreateMenuItem;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using KapeRest.Api.DTOs;


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

        [HttpPost("CreateMenuItem")]
        public async Task<IActionResult> CreateMenuItem([FromForm] CreateMenuItemDTOs dto)
        {
            if (dto.Image == null || dto.Image.Length == 0)
            {
                throw new Exception("Image cannot be null");
            }

            if(dto.Image == null || dto.Image.Length == 0)
                return BadRequest();

            using var ms = new MemoryStream();
            await dto.Image.CopyToAsync(ms);

            var products = string.IsNullOrEmpty(dto.ProductsJson)
                ? new List<MenuItemProductDTO>()
                : JsonSerializer.Deserialize<List<MenuItemProductDTO>>(dto.ProductsJson);

            var appDTO = new CreateMenuItemDTO
            {
                Item_name = dto.Item_name,
                Price = dto.Price,
                Description = dto.Description,
                Image = ms.ToArray(),
                Products = products
            };
           
            var result = await _menuItemService.CreateMenuItem(appDTO);
            return Ok(result);
        }


    }
}
