using System;

namespace KapeRest.Application.DTOs.Customers
{
    public class CreateCustomerDTO
    {
        public string Name { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class CustomerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
        public int TotalPurchases { get; set; }
        public decimal TotalSpent { get; set; }
        public int LoyaltyPoints { get; set; }
        public int LoyaltyLevel { get; set; }
        public int LoyaltyProgress { get; set; }  // Percentage (0-100)
    }

    public class CustomerSearchDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public int TotalPurchases { get; set; }
        public int LoyaltyPoints { get; set; }
        public int LoyaltyProgress { get; set; }  // Percentage (0-100)
    }
}
