using System.Collections.Generic;
using System.Threading.Tasks;
using KapeRest.Application.DTOs.Customers;

namespace KapeRest.Application.Interfaces.Customers
{
    public interface ICustomerService
    {
        Task<CustomerDTO> CreateCustomerAsync(CreateCustomerDTO dto);
        Task<CustomerDTO?> GetByIdAsync(int id);
        Task<CustomerDTO?> GetByContactNumberAsync(string contactNumber);
        Task<List<CustomerSearchDTO>> SearchCustomersAsync(string query);
        Task<List<CustomerDTO>> GetAllCustomersAsync();
        Task<bool> UpdatePurchaseStatsAsync(int customerId, decimal amount);
    }
}
