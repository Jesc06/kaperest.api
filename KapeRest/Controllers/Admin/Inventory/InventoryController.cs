using KapeRest.Application.DTOs.Admin.Inventory;
using KapeRest.Application.Services.Admin.Inventory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KapeRest.DTOs.Admin.Inventory;

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
        public async Task<ActionResult> AddProduct(API_CreateProductDTO addProduct)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            //Convert image file to byte array + get MIME type
            byte[] imageBytes = null;
            string imageMimeType = null;

            if (addProduct.ImageFile != null && addProduct.ImageFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await addProduct.ImageFile.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                    imageMimeType = addProduct.ImageFile.ContentType;
                }
            }

            var productDTO = new CreateProductDTO
            {
                ProductName = addProduct.ProductName,
                Category = addProduct.Category,
                Price = addProduct.Price,
                Quantity = addProduct.Quantity,
                ReorderLevel = addProduct.ReorderLevel,
                SupplierId = addProduct.SupplierId,
                Base64Image = imageBytes != null ? Convert.ToBase64String(imageBytes) : null,
                ImageMimeType = imageMimeType
            };
            var response = await _inventoryService.addProduct(productDTO);
            return Ok(response);
        }



    }
}
