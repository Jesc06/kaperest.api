using KapeRest.Application.DTOs.Admin.CreateMenuItem;
using KapeRest.Application.Interfaces.Admin.CreateMenuItem;
using KapeRest.Application.Interfaces.CurrentUserService;
using KapeRest.Domain.Entities.MenuEntities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Services.Admin.CreateMenuItem
{
    public class MenuItemService
    {
        private readonly IMenuItem _menuItem;
        private readonly ICurrentUser _currentUser;
        public MenuItemService(IMenuItem menuItem, ICurrentUser currentUser)
        {
            _menuItem = menuItem;
            _currentUser = currentUser;
        }

        public async Task<MenuItem> CreateMenuItem(CreateMenuItemDTO newItem)
        {
            string user = _currentUser.Email;
            string role = _currentUser.Role;

            return await _menuItem.CreateMenuItemAsync(user, role, newItem);
        }

        public async Task<MenuItem> UpdateMenuItem(int menuItemId, UpdateMenuItemDTO updatedItem)
        {
            return await _menuItem.UpdateMenuItemAsync(menuItemId, updatedItem);
        }

        public async Task<string> DeleteMenuItem(int id)
        {
            return await _menuItem.DeleteMenuItem(id);
        }

        public async Task<ICollection> GetAllMenuItem()
        {
            return await _menuItem.GetAllMenuItem();
        }

    }
}
