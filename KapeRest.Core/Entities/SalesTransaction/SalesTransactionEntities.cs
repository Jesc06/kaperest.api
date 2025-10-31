using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Core.Entities.SalesTransaction
{
    public class SalesTransactionEntities
    {
        public int Id { get; set; }
        public string ReceiptNumber { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        public DateTime DateTime { get; set; } = DateTime.Now;

        public string CashierId { get; set; } = string.Empty;  
        public int? BranchId { get; set; }                    

        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }

        public string PaymentMethod { get; set; } = "Cash";   
        public bool IsHold { get; set; } = false;
    }
}
