using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Domain.Entities.Inventory
{
    public class SupplierTransactionHistory
    {
        public int Id { get; set; }

        // Relationship
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        // Details
        public string ProductName { get; set; }
        public int QuantityDelivered { get; set; }
        public decimal TotalCost { get; set; }

        //Transaction date
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        //auto format date
        public string FormattedDate => TransactionDate.ToString("MM/dd/yyyy");
    }
}
