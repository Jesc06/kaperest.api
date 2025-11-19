using KapeRest.Core.Entities.MenuEntities;
using KapeRest.Domain.Entities.SupplierEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Domain.Entities.InventoryEntities
{
    public class ProductOfSupplier
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Stocks { get; set; }
        public string Units { get; set; }//"ml", "kg", "pcs"
        public decimal CostPrice { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string FormattedDate => TransactionDate.ToString("MM/dd/yyyy");

        // Relation
        public int SupplierId { get; set; }
        public AddSupplier Supplier { get; set; }

        public string CashierId { get; set; }
        public int? BranchId { get; set; }

        public ICollection<MenuItemProduct> MenuItemProducts { get; set; } = new List<MenuItemProduct>();
    }
}
