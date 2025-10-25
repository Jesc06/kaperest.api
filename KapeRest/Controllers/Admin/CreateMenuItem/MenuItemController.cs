using KapeRest.Application.Services.Admin.CreateMenuItem;
using KapeRest.Domain.Entities.MenuEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public async Task<IActionResult> CreateMenuItem([FromBody] CreateMenuItemDTO dto)
        {
            var result = await _menuItemService.CreateMenuItem(dto);
            return Ok(result);
        }


    }
}
