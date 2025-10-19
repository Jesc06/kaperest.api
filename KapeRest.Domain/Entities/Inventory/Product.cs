using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Domain.Entities.Inventory
{
    public class Product
    {
        public int Id { get; set; }
        public string imageURL { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public int ReorderLevel { get; set; }

        //time added new product
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public string FormattedDate => TransactionDate.ToString("MM/dd/yyyy");

        // Relation
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
    }
}
