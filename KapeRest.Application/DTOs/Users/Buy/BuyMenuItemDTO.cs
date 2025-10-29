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
        public int Quantity { get; set; }
        public string CashierId { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; } = 0;
        public decimal Tax { get; set; }
        public string PaymentMethod { get; set; } = "Cash";

    }
}
