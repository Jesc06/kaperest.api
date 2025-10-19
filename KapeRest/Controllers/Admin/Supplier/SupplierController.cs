using KapeRest.Application.DTOs.Admin.Supplier;
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
        public readonly AddSupplierService _supplierService;
        public SupplierController(AddSupplierService supplierService)
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
                SupplierName = addSuppliers.SupplierName,
                ContactPerson = addSuppliers.ContactPerson,
                Address = addSuppliers.Address,
            };

            var response = await _supplierService.addSupplier(supplierDTO);
            return Ok(response);
        }

        [HttpPut("UpdateSupplier")]
        public async Task<ActionResult> UpdateSupplier(UpdateSupplierDTO update)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _supplierService.UpdateSupplier(update);
            return Ok(response);
        }

        [HttpDelete("DeleteSupplier/{Id}")]
        public async Task<ActionResult> DeleteSupplier(int Id)
        {
            var result = await _supplierService.DeleteSupplier(Id);
            if (!result)
                return NotFound(new { Message = "Supplier not found or could not be deleted." });
            return Ok(new { Message = "Supplier deleted successfully." });
        }

    }
}
