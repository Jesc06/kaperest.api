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

            var cashierIdFromJWTClaims = User.FindFirst("cashierId")?.Value;//automatic get cashierId from JWT claims

            /*parang ang pinaka purpose ko dito sa cashierID jwt claims ay sa features na staff and cashier mag ka relationship kung
             baga kapag nag add ako ng item sa staff account automatically kung anong cashier ang naka tali sa staff acc
             sa cashier acc lang ma access o mapupunta yung created item na menu ng kaperest*/

            var appDTO = new CreateMenuItemDTO
            {
                Item_name = dto.Item_name,
                Price = dto.Price,
                Description = dto.Description,
                Image = ms.ToArray(),
                Products = products!,
                cashierId = cashierIdFromJWTClaims!
            };
           
            var result = await _menuItemService.CreateMenuItem(appDTO);
            return Ok(result);
        }

        [HttpPut("UpdateMenuItem")]
        public async Task<IActionResult> UpdateMenuItem([FromForm]UpdateMenuItemDTOs dto)
        {
            if (dto.Image == null || dto.Image.Length == 0)
            {
                throw new Exception("Image cannot be null");
            }
            using var ms = new MemoryStream();
            await dto.Image.CopyToAsync(ms);

            var products = string.IsNullOrEmpty(dto.ProductsJson)
                ? new List<MenuItemProductDTO>()
                : JsonSerializer.Deserialize<List<MenuItemProductDTO>>(dto.ProductsJson);
            var appDTO = new UpdateMenuItemDTO
            {
                Item_name = dto.Item_name,
                Price = dto.Price,
                Description = dto.Description,
                Image = ms.ToArray(),
                Products = products,
                cashierId = dto.cashierId,
            };
            var result = await _menuItemService.UpdateMenuItem(appDTO);
            return Ok(result);
        }

        [HttpDelete("DeleteMenuItem")]
        public async Task<ActionResult> DeleteMenuItem(string cashierId, int id)
        {
            var result = await _menuItemService.DeleteMenuItem(cashierId,id);
            return Ok(result);
        }

        [HttpGet("GetAllMenuItem")]
        public async Task<IActionResult> GetAllMenuItem(string cashierId)
        {
            var result = await _menuItemService.GetAllMenuItem(cashierId);
            return Ok(result);
        }


    }
}
