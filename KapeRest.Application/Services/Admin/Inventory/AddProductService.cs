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
    public class AddProductService
    {
        private readonly IInventory _inventory;
        public AddProductService(IInventory inventory)
        {
            _inventory = inventory;
        }
        public async Task<ProductResponseDTO> addProduct(CreateProductDTO add)
        {
            return await _inventory.AddProduct(add);
        }
        public async Task<ProductResponseDTO> UpdateProduct (UpdateProductDTO update)
        {
            return await _inventory.UpdateProduct(update);
        }

        public async Task<bool> DeleteProduct (int id)
        {
            return await _inventory.DeleteProduct(id);
        }


    }
}
