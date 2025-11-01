using KapeRest.Application.DTOs.Users.Sales;
using KapeRest.Application.Interfaces.Users.Sales;
using KapeRest.Core.Entities.SalesTransaction;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Infrastructure.Persistence.Repositories.Users.Sales
{
    public class SalesRepo : ISales 
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UsersIdentity> _userManager;
        public SalesRepo(ApplicationDbContext context, UserManager<UsersIdentity> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ICollection> GetSalesByCashiers(SalesDTO sales)
        {
            var cashier = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == sales.cashierId);
            if (cashier == null)
                throw new Exception("Cashier not found.");

            var data = await (from s in _context.SalesTransaction
                              join u in _context.UsersIdentity on s.CashierId equals u.Id
                              where s.CashierId == sales.cashierId && s.BranchId == u.BranchId
                              select new
                              {
                                  s.Id,
                                  CashierName = u.UserName,
                                  BranchName = u.Branch != null ? u.Branch.BranchName : "N/A",
                                  s.ReceiptNumber,
                                  s.DateTime,
                                  s.Subtotal,
                                  s.Tax,
                                  s.Discount,
                                  s.Total
                              }).ToListAsync();
            return data;
        }

        public async Task<ICollection> GetSalesByAdmin()
        {
            var sales = await (from s in _context.SalesTransaction
                              join u in _context.UsersIdentity on s.CashierId equals u.Id
                              join b in _context.Branches on u.BranchId equals b.Id into branchJoin
                              from bj in branchJoin.DefaultIfEmpty()
                              select new
                              {
                                  s.Id,
                                  CashierName = u.UserName,
                                  BranchName = bj != null ? bj.BranchName : "N/A",
                                  s.ReceiptNumber,
                                  s.DateTime,
                                  s.Subtotal,
                                  s.Tax,
                                  s.Discount,
                                  s.Total
                              }).ToListAsync();

            return sales;
        }



    }
}
