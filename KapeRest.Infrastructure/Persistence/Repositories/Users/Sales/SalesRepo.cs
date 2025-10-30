using KapeRest.Application.Interfaces.Users.Sales;
using KapeRest.Infrastructures.Persistence.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Infrastructure.Persistence.Repositories.Users.Sales
{
    public class SalesRepo 
    {
        private readonly ApplicationDbContext _context;
        public SalesRepo(ApplicationDbContext context)
        {
            _context = context;
        }



    }
}
