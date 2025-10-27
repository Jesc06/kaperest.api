using KapeRest.Application.DTOs.Admin.TaxDiscount;
using KapeRest.Core.Entities.Tax_Rate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.Admin.TaxDiscount
{
    public interface ITaxDiscount
    {
        Task<TaxAndRate> TaxDiscountAsync(TaxDiscountDTO dto);
        Task<string> UpdateTaxDiscount(UpdateTaxDiscountDTO dto);
        Task<ICollection> GetAllTaxAndDiscount();
        Task<string> DeleteTaxAndDiscount(int id);
    }
}
