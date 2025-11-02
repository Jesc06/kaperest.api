using KapeRest.Application.DTOs.Users.Sales;
using KapeRest.Application.Interfaces.Users.Sales;
using KapeRest.Core.Entities.SalesTransaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Services.Users.Sales
{
    public class SalesService
    {
        private readonly ISales _salesRepo;
        public SalesService(ISales salesRepo)
        {
            _salesRepo = salesRepo;
        }

        public async Task<ICollection<SalesReportDTO>> GetSalesByCashiers(string cashierId)
        {
         return await _salesRepo.GetSalesByCashiers(cashierId); 
        }
        public async Task<ICollection> GetSalesByAdmin(bool includeHold)
        {
            return await _salesRepo.GetSalesByAdmin(includeHold);
        }


    }
}
