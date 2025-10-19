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

        //helper method for converting image to base64
        private async Task<(string base64, string mimeType)> ConvertImageToBase64Async(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (null, null);

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var base64 = Convert.ToBase64String(ms.ToArray());
            return (base64, file.ContentType);
        }


        [HttpPost("AddProducts")]
        public async Task<ActionResult> AddProduct(API_CreateProductDTO addProduct)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var (base64Image, mimeType) = await ConvertImageToBase64Async(addProduct.ImageFile);

            var productDTO = new CreateProductDTO
            {
                ProductName = addProduct.ProductName,
                Category = addProduct.Category,
                Price = addProduct.Price,
                Quantity = addProduct.Quantity,
                ReorderLevel = addProduct.ReorderLevel,
                SupplierId = addProduct.SupplierId,
                Base64Image = base64Image,
                ImageMimeType = mimeType
            };
            var response = await _inventoryService.addProduct(productDTO);
            return Ok(response);
        }


        [HttpPut("UpdateProduct")]
        public async Task<ActionResult> UpdateProduct(API_UpdateProductDTO update) 
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var (base64Image, mimeType) = await ConvertImageToBase64Async(update.ImageFile);

            var updatedProduct = new UpdateProductDTO
            {
                Id = update.Id,
                ProductName = update.ProductName,
                Category = update.Category,
                Price = update.Price,
                Quantity = update.Quantity,
                ReorderLevel = update.ReorderLevel,
                Base64Image = base64Image,
                ImageMimeType = mimeType
            };

            var response = await _inventoryService.UpdateProduct(updatedProduct);
            return Ok(response);
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _inventoryService.DeleteProduct(id);
            return Ok(result);
        }



    }
}
