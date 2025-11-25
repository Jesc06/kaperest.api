using KapeRest.Application.Interfaces.Cashiers.Sales;
using KapeRest.Core.Entities.SalesTransaction;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Infrastructure.Persistence.Repositories.Cashiers.Sales
{
    public class SalesTransactionRepositories : IsalesTransaction
    {
        private readonly ApplicationDbContext _context;
        public SalesTransactionRepositories(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<SalesTransactionEntities>> Purchases()
        {
            var sales = await _context.SalesTransaction
            .OrderByDescending(s => s.DateTime)
            .ToListAsync();
            return sales;
        }   



    }
}
