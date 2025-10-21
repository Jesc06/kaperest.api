using KapeRest.Application.Services.Admin.CreateMenuItem;
using KapeRest.Domain.Entities.MenuEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KapeRest.DTOs.Admin.CreateMenuItem;
using KapeRest.Application.DTOs.Admin.CreateMenuItem;

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
        public async Task<ActionResult> CreateMenuItems([FromBody]CreateMenuItemDTO dto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _menuItemService.CreateMenuItem(dto);

            return Ok(new {  result.Id, result.ItemName,result.Price, Message = "Menu item created successfully and stock deducted." });

        }

    }
}
