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
        public int SupplierId { get; set; }
        public string User { get; set; }
        public string ProductName { get; set; }
        public string Price { get; set; }
        public int QuantityDelivered { get; set; }
        public decimal TotalCost { get; set; }
        public string Action { get; set; } = "Active";
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public string FormattedDate => TransactionDate.ToString("MMMM d, yyyy h:mm tt");
        public AddSupplier Supplier { get; set; }
    }
}
