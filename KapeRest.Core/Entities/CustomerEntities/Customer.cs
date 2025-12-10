using System;
using System.Collections.Generic;
using KapeRest.Core.Entities.VoucherEntities;

namespace KapeRest.Core.Entities.CustomerEntities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastPurchaseDate { get; set; }
        public int TotalPurchases { get; set; } = 0;
        public decimal TotalSpent { get; set; } = 0;
        
        // Loyalty Program
        public int LoyaltyPoints { get; set; } = 0;  // Points earned (1 purchase = 1 point)
        public int LoyaltyLevel { get; set; } = 0;   // How many times they completed the loyalty cycle

        // Navigation properties
        public virtual ICollection<VoucherUsage> VoucherUsages { get; set; } = new List<VoucherUsage>();
    }
}
