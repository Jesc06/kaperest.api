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
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public SalesTransactionEntities SalesTransaction { get; set; }
        public MenuItem MenuItem { get; set; }
    }
}
