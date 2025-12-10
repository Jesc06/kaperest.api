using KapeRest.Application.DTOs.Vouchers;
using KapeRest.Application.Interfaces.Vouchers;
using KapeRest.Core.Entities.VoucherEntities;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KapeRest.Infrastructure.Persistence.Repositories.Vouchers
{
    public class VoucherService : IVoucherService
    {
        private readonly ApplicationDbContext _context;

        public VoucherService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<VoucherDTO> CreateVoucherAsync(CreateVoucherDTO createVoucher, string createdBy)
        {
            // Check if code already exists
            var existingVoucher = await _context.Vouchers
                .FirstOrDefaultAsync(v => v.Code == createVoucher.Code);

            if (existingVoucher != null)
            {
                throw new Exception("Voucher code already exists");
            }

            var voucher = new Voucher
            {
                Code = createVoucher.Code.ToUpper(),
                DiscountPercent = createVoucher.DiscountPercent,
                MaxUses = createVoucher.MaxUses,
                CurrentUses = 0,
                IsActive = true,
                ExpiryDate = createVoucher.ExpiryDate,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy,
                Description = createVoucher.Description,
                CustomerId = createVoucher.CustomerId,
                IsCustomerSpecific = createVoucher.IsCustomerSpecific
            };

            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();

            return MapToDTO(voucher);
        }

        public async Task<VoucherValidationResultDTO> ValidateVoucherAsync(string code, decimal orderAmount, int? customerId = null)
        {
            var voucher = await _context.Vouchers
                .Include(v => v.VoucherUsages)
                .FirstOrDefaultAsync(v => v.Code == code.ToUpper());

            if (voucher == null)
            {
                return new VoucherValidationResultDTO
                {
                    IsValid = false,
                    Message = "Voucher code not found"
                };
            }

            if (!voucher.IsActive)
            {
                return new VoucherValidationResultDTO
                {
                    IsValid = false,
                    Message = "Voucher is inactive"
                };
            }

            // Check expiration first (highest priority)
            if (voucher.ExpiryDate.HasValue && voucher.ExpiryDate.Value < DateTime.UtcNow)
            {
                var expiredDate = voucher.ExpiryDate.Value.ToString("MMM dd, yyyy");
                return new VoucherValidationResultDTO
                {
                    IsValid = false,
                    Message = $"Voucher expired on {expiredDate}"
                };
            }

            // Check if voucher has been used at all (one-time use globally)
            if (voucher.CurrentUses >= voucher.MaxUses)
            {
                return new VoucherValidationResultDTO
                {
                    IsValid = false,
                    Message = "Voucher has already been used"
                };
            }

            // Check if this specific customer already used this voucher
            if (customerId.HasValue && voucher.VoucherUsages.Any(u => u.CustomerId == customerId.Value))
            {
                return new VoucherValidationResultDTO
                {
                    IsValid = false,
                    Message = "You have already used this voucher"
                };
            }

            var discountAmount = (orderAmount * voucher.DiscountPercent) / 100;

            return new VoucherValidationResultDTO
            {
                IsValid = true,
                Message = "Voucher is valid",
                DiscountPercent = voucher.DiscountPercent,
                DiscountAmount = discountAmount,
                VoucherId = voucher.Id,
                IsCustomerSpecific = voucher.IsCustomerSpecific,
                CustomerId = voucher.CustomerId
            };
        }

        public async Task<bool> UseVoucherAsync(int voucherId, int customerId, decimal originalAmount, decimal finalAmount, int? salesTransactionId = null)
        {
            var voucher = await _context.Vouchers.FindAsync(voucherId);

            if (voucher == null || voucher.CurrentUses >= voucher.MaxUses)
            {
                return false;
            }

            // Get customer by ID
            var customer = await _context.Customers.FindAsync(customerId);

            if (customer == null)
            {
                return false;
            }

            // If voucher is customer-specific, verify the customer
            if (voucher.IsCustomerSpecific && voucher.CustomerId.HasValue)
            {
                if (voucher.CustomerId.Value != customer.Id)
                {
                    return false;
                }
            }

            // Increment usage count
            voucher.CurrentUses++;

            // Record usage
            var usage = new VoucherUsage
            {
                VoucherId = voucherId,
                CustomerId = customer.Id,
                CustomerName = customer.Name,
                UsedDate = DateTime.UtcNow,
                SalesTransactionId = salesTransactionId,
                DiscountApplied = originalAmount - finalAmount,
                OriginalAmount = originalAmount,
                FinalAmount = finalAmount
            };

            _context.VoucherUsages.Add(usage);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<VoucherDTO?> GetVoucherByCodeAsync(string code)
        {
            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(v => v.Code == code.ToUpper());

            return voucher == null ? null : MapToDTO(voucher);
        }

        public async Task<List<VoucherDTO>> GetAllVouchersAsync()
        {
            var vouchers = await _context.Vouchers
                .OrderByDescending(v => v.CreatedDate)
                .ToListAsync();

            return vouchers.Select(MapToDTO).ToList();
        }

        public async Task<List<VoucherDTO>> GetActiveVouchersAsync()
        {
            var vouchers = await _context.Vouchers
                .Where(v => v.IsActive && 
                           v.CurrentUses < v.MaxUses &&
                           (!v.ExpiryDate.HasValue || v.ExpiryDate.Value > DateTime.UtcNow))
                .OrderByDescending(v => v.CreatedDate)
                .ToListAsync();

            return vouchers.Select(MapToDTO).ToList();
        }

        public async Task<bool> DeactivateVoucherAsync(int voucherId)
        {
            var voucher = await _context.Vouchers.FindAsync(voucherId);

            if (voucher == null)
            {
                return false;
            }

            voucher.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<VoucherUsageDTO>> GetVoucherUsageHistoryAsync(int voucherId)
        {
            var usages = await _context.VoucherUsages
                .Include(u => u.Voucher)
                .Where(u => u.VoucherId == voucherId)
                .OrderByDescending(u => u.UsedDate)
                .ToListAsync();

            return usages.Select(u => new VoucherUsageDTO
            {
                Id = u.Id,
                VoucherCode = u.Voucher.Code,
                CustomerName = u.CustomerName,
                UsedDate = u.UsedDate,
                DiscountApplied = u.DiscountApplied,
                OriginalAmount = u.OriginalAmount,
                FinalAmount = u.FinalAmount
            }).ToList();
        }

        public async Task<List<VoucherUsageDTO>> GetAllVoucherUsagesAsync()
        {
            var usages = await _context.VoucherUsages
                .Include(u => u.Voucher)
                .OrderByDescending(u => u.UsedDate)
                .ToListAsync();

            return usages.Select(u => new VoucherUsageDTO
            {
                Id = u.Id,
                VoucherCode = u.Voucher.Code,
                CustomerName = u.CustomerName,
                UsedDate = u.UsedDate,
                DiscountApplied = u.DiscountApplied,
                OriginalAmount = u.OriginalAmount,
                FinalAmount = u.FinalAmount
            }).ToList();
        }

        private static VoucherDTO MapToDTO(Voucher voucher)
        {
            return new VoucherDTO
            {
                Id = voucher.Id,
                Code = voucher.Code,
                DiscountPercent = voucher.DiscountPercent,
                MaxUses = voucher.MaxUses,
                CurrentUses = voucher.CurrentUses,
                IsActive = voucher.IsActive,
                ExpiryDate = voucher.ExpiryDate,
                CreatedDate = voucher.CreatedDate,
                CreatedBy = voucher.CreatedBy,
                Description = voucher.Description,
                CustomerId = voucher.CustomerId,
                IsCustomerSpecific = voucher.IsCustomerSpecific
            };
        }
    }
}
