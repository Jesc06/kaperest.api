using KapeRest.Api.DTOs.AddProducts;
using KapeRest.Application.DTOs.Admin.Inventory;
using KapeRest.Application.Services.Admin.Inventory;
using KapeRest.Domain.Entities.InventoryEntities;
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
        public async Task<ActionResult> AddProductOfSuppliers(InventoryDTO add)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var cashierIDFromJwtClaims = User.FindFirst("cashierId")?.Value;
            var cashierBranchIdFromJwtClaims = User.FindFirst("branchId")?.Value;

            var addProduct = new CreateProductDTO
            {
                ProductName = add.ProductName,
                CostPrice = add.CostPrice,
                Stocks = add.Stocks,
                Units = add.Units,
                SupplierId = add.SupplierId,
                CashierId = cashierIDFromJwtClaims!,
                BranchId = cashierBranchIdFromJwtClaims != null ? int.Parse(cashierBranchIdFromJwtClaims) : null
            };

            var response = await _inventoryService.AddProductOfSuppliers(addProduct);
            return Ok(response);
        }

        [HttpPut("UpdateProductOfSuppliers")]
        public async Task<ActionResult> UpdateProductOfSuppliers(UpdateProductDTO update) 
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _inventoryService.UpdateProductOfSuppliers(update);
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

        [HttpGet("GetAllProducts")]
        public async Task<ActionResult> GetAllProducts()
        {
            var products = await _inventoryService.GetAllProducts();
            return Ok(products);
        }



    }
}
