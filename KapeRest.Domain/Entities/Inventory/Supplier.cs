using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Domain.Entities.Inventory
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }

        // Navigation (one-to-many)
        public ICollection<Product> Products { get; set; }

        //Transaction history per supplier
        public ICollection<SupplierTransactionHistory> TransactionHistories { get; set; }
    }
}
