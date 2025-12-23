using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KapeRest.Application.DTOs.Customers;
using KapeRest.Application.Interfaces.Customers;
using KapeRest.Core.Entities.CustomerEntities;
using KapeRest.Core.Entities.VoucherEntities;
using KapeRest.Infrastructure.Persistence.Database;
using KapeRest.Infrastructure.Persistence.Database;

namespace KapeRest.Infrastructure.Persistence.Repositories.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CustomerDTO> CreateCustomerAsync(CreateCustomerDTO dto)
        {
            // Check if customer with same contact number already exists
            var existing = await _context.Customers
                .FirstOrDefaultAsync(c => c.ContactNumber == dto.ContactNumber);

            if (existing != null)
            {
                return MapToDTO(existing);
            }

            var customer = new Customer
            {
                Name = dto.Name,
                ContactNumber = dto.ContactNumber,
                Email = dto.Email,
                CreatedDate = DateTime.UtcNow
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return MapToDTO(customer);
        }

        public async Task<CustomerDTO?> GetByIdAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            return customer != null ? MapToDTO(customer) : null;
        }

        public async Task<CustomerDTO?> GetByContactNumberAsync(string contactNumber)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.ContactNumber == contactNumber);
            return customer != null ? MapToDTO(customer) : null;
        }

        public async Task<List<CustomerSearchDTO>> SearchCustomersAsync(string query)
        {
            query = query.ToLower();
            const int LOYALTY_THRESHOLD = 10;

            var customers = await _context.Customers
                .Where(c => c.Name.ToLower().Contains(query) ||
                           c.ContactNumber.Contains(query))
                .OrderByDescending(c => c.LastPurchaseDate)
                .Take(10)
                .Select(c => new CustomerSearchDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    ContactNumber = c.ContactNumber,
                    TotalPurchases = c.TotalPurchases,
                    LoyaltyPoints = c.LoyaltyPoints,
                    LoyaltyProgress = (c.LoyaltyPoints * 100) / LOYALTY_THRESHOLD
                })
                .ToListAsync();

            return customers;
        }

        public async Task<List<CustomerDTO>> GetAllCustomersAsync()
        {
            var customers = await _context.Customers
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();

            return customers.Select(MapToDTO).ToList();
        }

        public async Task<bool> UpdatePurchaseStatsAsync(int customerId, decimal amount)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null) return false;

            customer.TotalPurchases++;
            customer.TotalSpent += amount;
            customer.LastPurchaseDate = DateTime.UtcNow;
            customer.LoyaltyPoints++;

            // Check if customer completed loyalty cycle (10 purchases = 100%)
            const int LOYALTY_THRESHOLD = 10;
            if (customer.LoyaltyPoints >= LOYALTY_THRESHOLD)
            {
                // Generate voucher for customer
                await GenerateLoyaltyVoucherAsync(customer);
                
                // Reset points, increase level
                customer.LoyaltyPoints = 0;
                customer.LoyaltyLevel++;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private async Task GenerateLoyaltyVoucherAsync(Customer customer)
        {
            // Calculate tiered discount based on loyalty level
            // Level 1 (0): 10%
            // Level 2 (1): 20%
            // Level 3+ (2+): 30%
            int discountPercent = customer.LoyaltyLevel switch
            {
                0 => 10,  // First reward
                1 => 20,  // Second reward
                _ => 30   // Third reward onwards (max)
            };

            // Create a customer-specific voucher with tiered discount
            var voucher = new Voucher
            {
                Code = $"LOYAL-{customer.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}",
                DiscountPercent = discountPercent,
                MaxUses = 1,
                CurrentUses = 0,
                IsActive = true,
                ExpiryDate = DateTime.UtcNow.AddMonths(3),
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System",
                Description = $"Loyalty Reward for {customer.Name} - Level {customer.LoyaltyLevel + 1} ({discountPercent}% OFF)",
                CustomerId = customer.Id,
                IsCustomerSpecific = true
            };

            _context.Vouchers.Add(voucher);
        }

        private CustomerDTO MapToDTO(Customer customer)
        {
            const int LOYALTY_THRESHOLD = 10;
            int progress = (customer.LoyaltyPoints * 100) / LOYALTY_THRESHOLD;
            
            return new CustomerDTO
            {
                Id = customer.Id,
                Name = customer.Name,
                ContactNumber = customer.ContactNumber,
                Email = customer.Email,
                CreatedDate = customer.CreatedDate,
                LastPurchaseDate = customer.LastPurchaseDate,
                TotalPurchases = customer.TotalPurchases,
                TotalSpent = customer.TotalSpent,
                LoyaltyPoints = customer.LoyaltyPoints,
                LoyaltyLevel = customer.LoyaltyLevel,
                LoyaltyProgress = progress
            };
        }
    }
}
