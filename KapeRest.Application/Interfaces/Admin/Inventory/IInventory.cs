using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Application.DTOs.Admin.Inventory;


namespace KapeRest.Application.Interfaces.Admin.Inventory
{
    public interface IInventory
    {
        Task<string> AddProductOfSuppliers(string currentUser,string role,CreateProductDTO addProduct);
        Task<string> UpdateProductOfSuppliers(string currentUser,string role,UpdateProductDTO update);
        Task<bool> DeleteProductOfSuppliers(string currentUser,string role,int productId);
        Task<ICollection> GetAllProducts(string userID);
        Task<ICollection> GetAllProducts_Admin();
    }
}
