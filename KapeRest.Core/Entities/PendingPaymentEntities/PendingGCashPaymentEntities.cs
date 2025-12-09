using System;
using System.ComponentModel.DataAnnotations;

namespace KapeRest.Core.Entities.PendingPaymentEntities
{
    public class PendingGCashPaymentEntities
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string PaymentReference { get; set; } = string.Empty;
        
        public string? CashierId { get; set; }
        
        public int? BranchId { get; set; }
        
        public decimal DiscountPercent { get; set; }
        
        public decimal TaxPercent { get; set; }
        
        public decimal TotalAmount { get; set; }
        
        public string CartItemsJson { get; set; } = string.Empty; // Store cart items as JSON
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public bool IsCompleted { get; set; } = false;
    }
}
