using KapeRest.Application.DTOs.Admin.Supplier;
using KapeRest.Application.Interfaces.Admin.Inventory;
using KapeRest.Application.Interfaces.Admin.Supplier;
using KapeRest.Application.Interfaces.CurrentUserService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Services.Admin.Supplier
{
    public class AddSupplierService
    {
        private readonly ISupplier _supplier;
        private ICurrentUser _currentUser;
        public AddSupplierService(ISupplier supplier, ICurrentUser currentUser)
        {
            _supplier = supplier;
            _currentUser = currentUser;
        }
        public async Task<SupplierResponseDTO> addSupplier(CreateSupplierDTO add)
        {
            var user = _currentUser.Email;
            var role = _currentUser.Role;
            return await _supplier.AddSupplier(add,user,role);
        }

        public async Task<SupplierResponseDTO> UpdateSupplier(UpdateSupplierDTO update, string userId)
        {
            return await _supplier.UpdateSupplier(update,userId);
        }

        public async Task<string> DeleteSupplier(int supplierId, string userId, string role)
        {
            return await _supplier.DeleteSupplier(supplierId, userId, role);
        }

        public async Task<ICollection> GetAllSuppliers(string userId)
        {
            return await _supplier.GetAllSupplier(userId);
        }

    }
}
