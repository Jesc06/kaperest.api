using KapeRest.Application.DTOs.Admin.CreateMenuItem;
using KapeRest.Domain.Entities.MenuEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.Admin.CreateMenuItem
{
    public interface IMenuItem
    {
        Task<MenuItem> CreateMenuItem(string currentUser, string role, CreateMenuItemDTO dto);
    }
}
