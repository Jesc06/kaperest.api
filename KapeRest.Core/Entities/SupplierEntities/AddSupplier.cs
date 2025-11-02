using KapeRest.Domain.Entities.InventoryEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Domain.Entities.SupplierEntities
{
    public class AddSupplier
    {
        public int Id { get; set; }
        public string SupplierName { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string FormattedDate => TransactionDate.ToString("MM/dd/yyyy");

        public ICollection<ProductOfSupplier> Products { get; set; } = new List<ProductOfSupplier>();

        public ICollection<SupplierTransactionHistory> TransactionHistories { get; set; }
    }
}
