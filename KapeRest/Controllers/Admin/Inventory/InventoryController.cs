using KapeRest.Application.DTOs.Admin.Inventory;
using KapeRest.Application.Services.Admin.Inventory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KapeRest.Controllers.Admin.Inventory
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryService _inventoryService;
        public InventoryController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpPost("AddProducts")]
        public async Task<ActionResult> AddProduct(CreateProductDTO addProduct)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var productDTO = new CreateProductDTO
            {
                ProductName = addProduct.ProductName,
                Category = addProduct.Category,
                Price = addProduct.Price,
                Quantity = addProduct.Quantity,
                ReorderLevel = addProduct.ReorderLevel,
                SupplierId = addProduct.SupplierId,
            };
            var response = await _inventoryService.addProduct(productDTO);
            return Ok(response);
        }



    }
}
