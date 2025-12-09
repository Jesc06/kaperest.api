using System;
using System.Collections.Generic;

namespace KapeRest.Application.DTOs.PayMongo
{
    public class PendingPaymentDTO
    {
        public string PaymentReference { get; set; } = string.Empty;
        public string? CashierId { get; set; }
        public int? BranchId { get; set; }
        public List<CartItemDTO> CartItems { get; set; } = new List<CartItemDTO>();
        public decimal DiscountPercent { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class CartItemDTO
    {
        public int MenuItemId { get; set; }
        public int? MenuItemSizeId { get; set; } // Size ID for the menu item
        public string? Size { get; set; } // Size name (Small, Medium, Large, Regular)
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? SugarLevel { get; set; } // Sugar level preference for GCash payments
    }
}
