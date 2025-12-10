using System;

namespace KapeRest.Application.DTOs.Vouchers
{
    public class CreateVoucherDTO
    {
        public string Code { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public int MaxUses { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public bool IsCustomerSpecific { get; set; } = false;
    }

    public class VoucherDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public int MaxUses { get; set; }
        public int CurrentUses { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public bool IsCustomerSpecific { get; set; }
    }

    public class ValidateVoucherDTO
    {
        public string Code { get; set; } = string.Empty;
        public decimal OrderAmount { get; set; }
    }

    public class VoucherValidationResultDTO
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public int VoucherId { get; set; }
        public bool IsCustomerSpecific { get; set; }
        public int? CustomerId { get; set; }
    }

    public class VoucherUsageDTO
    {
        public int Id { get; set; }
        public string VoucherCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime UsedDate { get; set; }
        public decimal DiscountApplied { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal FinalAmount { get; set; }
    }
}
