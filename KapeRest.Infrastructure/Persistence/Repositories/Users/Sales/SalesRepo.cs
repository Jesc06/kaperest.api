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

        public async Task<IEnumerable<SalesTransactionEntities>> GetSalesByCashiers(SalesDTO sales)
        {
            var cashier = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == sales.cashierId);
            if (cashier == null)
                throw new Exception("Cashier not found.");

            return await _context.SalesTransaction
                .Where(x => x.CashierId == cashier.Id && x.BranchId == cashier.BranchId)
                .ToListAsync();
        }

    }
}
