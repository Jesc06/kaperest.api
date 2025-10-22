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
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            if(dto.image == null || dto.image.Length == 0)
                return BadRequest();


            using var ms = new MemoryStream();
            await dto.image.CopyToAsync(ms); 
            byte[] imageData = ms.ToArray();

            var response = new CreateMenuItemDTO
            {
               ItemName = dto.ItemName,
               Price = dto.Price,
               Description = dto.Description,
               image = imageData,
               Products = dto.Products  
            };

            var result = await _menuItemService.CreateMenuItem(response);

            return Ok(new {  result.Id, result.ItemName,result.Price, result.image, Message = "Menu item created successfully," });

        }

    }
}
