using KapeRest.Domain.Entities.InventoryEntities;
using KapeRest.Domain.Entities.SupplierEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Domain.Entities.SupplierEntities
{
    public class Supplier
    {
        public int Id { get; set; }
        public string SupplierName { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public string FormattedDate => TransactionDate.ToString("MM/dd/yyyy");

        // Navigation (one-to-many)
        public ICollection<Product> Products { get; set; }

        //Transaction history per supplier
        public ICollection<SupplierTransactionHistory> TransactionHistories { get; set; }
    }
}
