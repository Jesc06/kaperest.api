using KapeRest.Application.DTOs.Admin.CreateMenuItem;
using KapeRest.Domain.Entities.MenuEntities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.Admin.CreateMenuItem
{
    public interface IMenuItem
    {
        Task<MenuItem> CreateMenuItemAsync(string user, string role, CreateMenuItemDTO dto);
        Task<MenuItem> UpdateMenuItemAsync(UpdateMenuItemDTO dto);
        Task<string> DeleteMenuItem(string cashierId, int id);
        Task<ICollection> GetAllMenuItem(string cashierId);
    }
}
