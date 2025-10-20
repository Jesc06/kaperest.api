using KapeRest.Application.DTOs.Admin.Inventory;
using KapeRest.Application.Services.Admin.Inventory;
using KapeRest.Domain.Entities.InventoryEntities;
using KapeRest.DTOs.Admin.Inventory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KapeRest.Controllers.Admin.Inventory
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly AddProductService _inventoryService;
        public InventoryController(AddProductService inventoryService)
        {
            _inventoryService = inventoryService;
        }


        [HttpPost("AddProductsOfSuppliers")]
        public async Task<ActionResult> AddProductOfSuppliers(API_CreateProductDTO addProduct)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var productDTO = new CreateProductDTO
            {
                ProductName = addProduct.ProductName,
                Category = addProduct.Category,
                Price = addProduct.Price,
                Stock = addProduct.Stock,
                SupplierId = addProduct.SupplierId,
            };
            var response = await _inventoryService.AddProductOfSuppliers(productDTO);
            return Ok(response);
        }


        [HttpPut("UpdateProductOfSuppliers")]
        public async Task<ActionResult> UpdateProductOfSuppliers(API_UpdateProductDTO update) 
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedProduct = new UpdateProductDTO
            {
                Id = update.Id,
                ProductName = update.ProductName,
                Category = update.Category,
                Price = update.Price,
                Stock = update.Stock,
            };

            var response = await _inventoryService.UpdateProductOfSuppliers(updatedProduct);
            return Ok(response);
        }

        [HttpDelete("DeleteProductOfSuppliers/{id}")]
        public async Task<ActionResult> DeleteProductOfSuppliers(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _inventoryService.DeleteProductOfSuppliers(id);
            return Ok(result);
        }



    }
}
