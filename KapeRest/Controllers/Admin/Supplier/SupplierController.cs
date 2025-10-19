using KapeRest.Application.DTOs.Admin.Inventory;
using KapeRest.Application.Services.Admin.Inventory;
using KapeRest.Application.Services.Admin.Supplier;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KapeRest.Controllers.Admin.Supplier
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        public readonly SupplierService _supplierService;
        public SupplierController(SupplierService supplierService)
        {
            _supplierService = supplierService;
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

            var response = await _supplierService.addSupplier(supplierDTO);
            return Ok(response);
        }
    }
}
