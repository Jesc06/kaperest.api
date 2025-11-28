    using KapeRest.Application.Services.Admin.CreateMenuItem;
    using KapeRest.Domain.Entities.MenuEntities;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using KapeRest.Application.DTOs.Admin.CreateMenuItem;
    using Microsoft.AspNetCore.Http;
    using System.IO;
    using System.Threading.Tasks;
    using System.Text.Json;
    using KapeRest.Api.DTOs.MenuItem;


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
            // Remove the throw Exception, use proper validation
            if (dto.Image == null || dto.Image.Length == 0)
            {
                return BadRequest(new { message = "Image is required" });
            }

            try
            {
                using var ms = new MemoryStream();
                await dto.Image.CopyToAsync(ms);

                var products = string.IsNullOrEmpty(dto.ProductsJson)
                    ? new List<MenuItemProductDTO>()
                    : JsonSerializer.Deserialize<List<MenuItemProductDTO>>(dto.ProductsJson);

                var cashierIdFromJWTClaims = User.FindFirst("cashierId")?.Value;

                if (string.IsNullOrEmpty(cashierIdFromJWTClaims))
                {
                    return Unauthorized(new { message = "Cashier ID not found in token" });
                }

                var appDTO = new CreateMenuItemDTO
                {
                    Item_name = dto.Item_name,
                    Price = dto.Price,
                    Category = dto.Category,
                    Description = dto.Description,
                    Image = ms.ToArray(),
                    Products = products!,
                    cashierId = cashierIdFromJWTClaims!,
                    IsAvailable = "Yes"
                };

                var result = await _menuItemService.CreateMenuItem(appDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Proper error handling
                return StatusCode(500, new
                {
                    message = "Failed to create menu item",
                    error = ex.Message
                });
            }
        }

        [HttpPut("UpdateMenuItem")]
            public async Task<IActionResult> UpdateMenuItem([FromForm] UpdateMenuItemDTOs dto)
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
                    Id = dto.Id, 
                    cashierId = dto.cashierId,
                    Item_name = dto.Item_name,
                    Price = dto.Price,
                    Category = dto.Category,
                    Description = dto.Description,
                    Image = ms.ToArray(),
                    IsAvailable = dto.IsAvailable, 
                    Products = products
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
