using System;
using KapeRest.Core.Entities.SalesTransaction;
using KapeRest.Core.Entities.CustomerEntities;

namespace KapeRest.Core.Entities.VoucherEntities
{
    public class VoucherUsage
    {
        public int Id { get; set; }
        public int VoucherId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime UsedDate { get; set; } = DateTime.UtcNow;
        public int? SalesTransactionId { get; set; }
        public decimal DiscountApplied { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal FinalAmount { get; set; }

        // Navigation properties
        public virtual Voucher Voucher { get; set; } = null!;
        public virtual Customer Customer { get; set; } = null!;
        public virtual SalesTransactionEntities? SalesTransaction { get; set; }
    }
}
