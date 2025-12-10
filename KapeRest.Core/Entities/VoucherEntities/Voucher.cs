using System;
using System.Collections.Generic;

namespace KapeRest.Core.Entities.VoucherEntities
{
    public class Voucher
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public int MaxUses { get; set; }
        public int CurrentUses { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime? ExpiryDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        // Customer-specific voucher
        public int? CustomerId { get; set; }  // Null means voucher is for everyone
        public bool IsCustomerSpecific { get; set; } = false;

        // Navigation properties
        public virtual ICollection<VoucherUsage> VoucherUsages { get; set; } = new List<VoucherUsage>();
    }
}
