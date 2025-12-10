using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using KapeRest.Application.DTOs.Customers;
using KapeRest.Application.Interfaces.Customers;

namespace KapeRest.Api.Controllers.Customers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.ContactNumber))
            {
                return BadRequest(new { message = "Name and contact number are required" });
            }

            var customer = await _customerService.CreateCustomerAsync(dto);
            return Ok(customer);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound(new { message = "Customer not found" });
            }

            return Ok(customer);
        }

        [HttpGet("GetByContactNumber/{contactNumber}")]
        public async Task<IActionResult> GetByContactNumber(string contactNumber)
        {
            var customer = await _customerService.GetByContactNumberAsync(contactNumber);
            if (customer == null)
            {
                return NotFound(new { message = "Customer not found" });
            }

            return Ok(customer);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> SearchCustomers([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { message = "Search query is required" });
            }

            var customers = await _customerService.SearchCustomersAsync(query);
            return Ok(customers);
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }
    }
}
