using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Domain.Entities.SupplierEntities
{
    public class SupplierTransactionHistory
    {
        public int Id { get; set; }

        // Relationship
        public int SupplierId { get; set; }
        public AddSupplier Supplier { get; set; }

        // Details
        public string ProductName { get; set; }
        public int QuantityDelivered { get; set; }
        public decimal TotalCost { get; set; }

        // Transaction status (Added, Updated, Deleted)
        public string Status { get; set; } = "Active";

        // Transaction date
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        // Formatted date (auto)
        public string FormattedDate => TransactionDate.ToString("MMMM d, yyyy h:mm tt");
    }
}
