using KapeRest.Application.DTOs.Admin.Inventory;
using KapeRest.Application.Interfaces.Admin.Inventory;
using KapeRest.Application.Interfaces.CurrentUserService;
using System;
using System.Collections;
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
        public async Task<string> AddProductOfSuppliers(CreateProductDTO add)
        {
            var currentActiveUser = _currentUser.Email;
            var role = _currentUser.Role;

            if (string.IsNullOrEmpty(currentActiveUser))
                throw new UnauthorizedAccessException("User is not authenticated.");

            return await _inventory.AddProductOfSuppliers(currentActiveUser,role, add);
        }

        public async Task<string> UpdateProductOfSuppliers (UpdateProductDTO update)
        {
            var currentActiveUser = _currentUser.Email;
            var role = _currentUser.Role;

            if (string.IsNullOrEmpty(currentActiveUser))
                throw new UnauthorizedAccessException("User is not authenticated.");

            return await _inventory.UpdateProductOfSuppliers(currentActiveUser,role, update);
        }

        public async Task<bool> DeleteProductOfSuppliers (int id)
        {
            var currentActiveUser = _currentUser.Email;
            var role = _currentUser.Role;

            if (string.IsNullOrEmpty(currentActiveUser))
                throw new UnauthorizedAccessException("User is not authenticated.");

            return await _inventory.DeleteProductOfSuppliers(currentActiveUser,role, id);
        }

        public async Task<ICollection> GetAllProducts(string userId)
        {
            return await _inventory.GetAllProducts(userId);
        }


        public async Task<ICollection> GetAllProducts_Admin()
        {
            return await _inventory.GetAllProducts_Admin();
        }


    }
}
