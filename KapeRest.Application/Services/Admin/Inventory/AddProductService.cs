using KapeRest.Application.DTOs.Admin.Inventory;
using KapeRest.Application.Interfaces.Admin.Inventory;
using KapeRest.Application.Interfaces.CurrentUserService;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Services.Admin.Inventory
{
    public class AddProductService
    {
        private readonly IInventory _inventory;
        private ICurrentUser _currentUser;
        public AddProductService(IInventory inventory, ICurrentUser currentUser)
        {
            _inventory = inventory;
            _currentUser = currentUser;
        }
        public async Task<ProductResponseDTO> addProduct(CreateProductDTO add)
        {
            var currentActiveUser = _currentUser.Email;

            if (string.IsNullOrEmpty(currentActiveUser))
                throw new UnauthorizedAccessException("User is not authenticated.");

            return await _inventory.AddProduct(currentActiveUser, add);
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
