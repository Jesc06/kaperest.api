using KapeRest.Application.Interfaces.Cashiers.Sales;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Infrastructure.Persistence.Repositories.Cashiers.Sales
{
    public class OverAllSalesRepository : IOverallSales
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UsersIdentity> _userManager;
        public OverAllSalesRepository(ApplicationDbContext context, UserManager<UsersIdentity> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ICollection> GetAllSalesByCashiers(string cashierId)
        {
            var cashier = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == cashierId);
            if (cashier == null)
                throw new Exception("Cashier not found.");

            var data = await (from s in _context.SalesTransaction
                              join u in _context.UsersIdentity on s.CashierId equals u.Id
                              where s.CashierId == cashierId && s.BranchId == u.BranchId
                              select new
                              {
                                  s.Id,
                                  s.MenuItemName,
                                  s.DateTime,
                                  s.Subtotal,
                                  s.Tax,
                                  s.Discount,
                                  s.Total,
                                  s.Status
                              }).ToListAsync();
            return data;
        }


        public async Task<ICollection> GetAllSalesByAdmin()
        {
            var sales = await (from s in _context.SalesTransaction
                               join u in _context.UsersIdentity on s.CashierId equals u.Id
                               join b in _context.Branches on u.BranchId equals b.Id into branchJoin
                               from bj in branchJoin.DefaultIfEmpty()
                               select new
                               {
                                   s.Id,
                                   CashierName = u.UserName,
                                   BranchName = u.Branch != null ? u.Branch.BranchName : "N/A",
                                   BranchLocation = u.Branch != null ? u.Branch.Location : "N/A",
                                   Email = u.Email != null ? u.Email : "N/A",
                                   s.MenuItemName,
                                   s.DateTime,
                                   s.Subtotal,
                                   s.Tax,
                                   s.Discount,
                                   s.Total,
                                   s.Status
                               }).ToListAsync();

            return sales;
        }




    }
}
