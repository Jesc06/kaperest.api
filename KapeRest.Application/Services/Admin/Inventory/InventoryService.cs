using KapeRest.Application.DTOs.Admin.Inventory;
using KapeRest.Application.Interfaces.Admin.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Services.Admin.Inventory
{
    public class InventoryService
    {
        private readonly IInventory _inventory;
        public InventoryService(IInventory inventory)
        {
            _inventory = inventory;
        }

        public async Task<SupplierResponseDTO> addSupplier(CreateSupplierDTO add)
        {
            return await _inventory.AddSupplier(add);
        }
        public async Task<ProductResponseDTO> addProduct(CreateProductDTO add)
        {
            return await _inventory.AddProduct(add);
        }



    }
}
