using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.DTOs.Users.Buy
{
    public class BuyMenuItemDTO
    {
        public int MenuItemId { get; set; }
        public int? MenuItemSizeId { get; set; } // Optional: specific size variation
        public string? Size { get; set; } // Size selection (Small, Medium, Large) - nullable for products without sizes
        public string? SugarLevel { get; set; } // Sugar level selection (100%, 75%, 50%, 25%, 0%) - nullable
        public int Quantity { get; set; }
        public string CashierId { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; } = 0;
        public decimal Tax { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string? VoucherCode { get; set; } // Voucher code for automatic discount
        public string? CustomerName { get; set; } // Customer name for voucher tracking
        public int? CustomerId { get; set; } // Customer ID for purchase tracking

    }
}
