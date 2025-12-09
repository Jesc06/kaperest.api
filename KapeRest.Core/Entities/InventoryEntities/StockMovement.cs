using System;

namespace KapeRest.Domain.Entities.InventoryEntities
{
    public class StockMovement
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ProductOfSupplier Product { get; set; }
        public string MovementType { get; set; } // "Stock In" or "Stock Out"
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Reason { get; set; } // "Purchase", "Sale", "Return", "Adjustment", etc.
        public DateTime TransactionDate { get; set; }
        public string UserId { get; set; }
        public int? BranchId { get; set; }
    }
}
