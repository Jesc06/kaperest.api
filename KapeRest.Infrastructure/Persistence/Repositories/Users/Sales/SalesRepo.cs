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

        public async Task<ICollection<SalesReportDTO>> GetSalesByCashiers(string cashierId)
        {
            var cashier = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == cashierId);
            if (cashier == null)
                throw new Exception("Cashier not found.");

            var data = await (from s in _context.SalesTransaction
                              join u in _context.UsersIdentity on s.CashierId equals u.Id
                              where s.CashierId == cashierId && s.BranchId == u.BranchId
                              select new SalesReportDTO
                              {
                                  Id = s.Id,
                                  CashierName = u.UserName,
                                  BranchName = u.Branch != null ? u.Branch.BranchName : "N/A",
                                  ReceiptNumber =  s.ReceiptNumber,
                                  DateTime = s.DateTime,
                                  Subtotal = s.Subtotal,
                                  Tax = s.Tax,
                                  Discount = s.Discount,
                                  Total = s.Total,
                                  Status = s.Status
                              }).ToListAsync();
            return data;
        }

        public async Task<ICollection> GetSalesByAdmin(bool includeHold = false)
        {
            var query = from s in _context.SalesTransaction
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
                            s.Total,
                            s.Status
                        };

            if (!includeHold)
                query = query.Where(s => s.Status == "Completed" || s.Status == "Canceled");

            return await query.ToListAsync();
        }



    }
}
