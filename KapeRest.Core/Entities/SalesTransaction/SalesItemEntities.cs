using KapeRest.Domain.Entities.MenuEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Core.Entities.SalesTransaction
{
    public class SalesItemEntities
    {
        public int Id { get; set; }
        public int SalesTransactionId { get; set; }
        public int? MenuItemId { get; set; }
        public int? MenuItemSizeId { get; set; } // Track which size was ordered
        public string Size { get; set; } // Store size for historical record (Small, Medium, Large)
        public string SugarLevel { get; set; } // Store sugar level (100%, 75%, 50%, 25%, 0%)
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public SalesTransactionEntities SalesTransaction { get; set; }
        public MenuItem MenuItem { get; set; }
    }
}
