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
                Name = addProduct.Name,
                Category = addProduct.Category,
                Price = addProduct.Price,
                Quantity = addProduct.Quantity,
                ReorderLevel = addProduct.ReorderLevel,
                SupplierId = addProduct.SupplierId,
            };
            var response = await _inventoryService.addProduct(productDTO);
            return Ok(response);
        }

        [HttpPost("AddSuppliers")]
        public async Task<ActionResult> AddSuppliers(CreateSupplierDTO addSuppliers)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var supplierDTO = new CreateSupplierDTO
            {
                Name = addSuppliers.Name,
                Contact = addSuppliers.Contact,
                Address = addSuppliers.Address,
            };

            var response = await _inventoryService.addSupplier(supplierDTO);
            return Ok(response);
        }


    }
}
