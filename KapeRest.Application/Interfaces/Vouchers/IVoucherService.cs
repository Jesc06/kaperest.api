using KapeRest.Application.DTOs.Vouchers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.Vouchers
{
    public interface IVoucherService
    {
        Task<VoucherDTO> CreateVoucherAsync(CreateVoucherDTO createVoucher, string createdBy);
        Task<VoucherValidationResultDTO> ValidateVoucherAsync(string code, decimal orderAmount, int? customerId = null);
        Task<bool> UseVoucherAsync(int voucherId, int customerId, decimal originalAmount, decimal finalAmount, int? salesTransactionId = null);
        Task<VoucherDTO?> GetVoucherByCodeAsync(string code);
        Task<List<VoucherDTO>> GetAllVouchersAsync();
        Task<List<VoucherDTO>> GetActiveVouchersAsync();
        Task<bool> DeactivateVoucherAsync(int voucherId);
        Task<List<VoucherUsageDTO>> GetVoucherUsageHistoryAsync(int voucherId);
        Task<List<VoucherUsageDTO>> GetAllVoucherUsagesAsync();
    }
}
