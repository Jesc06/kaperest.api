using KapeRest.Application.DTOs.Admin.TaxDiscount;
using KapeRest.Application.Interfaces.Admin.TaxDiscount;
using KapeRest.Core.Entities.Tax_Rate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Services.Admin.TaxDiscount
{
    public class TaxDiscountService
    {
        private readonly ITaxDiscount _taxDiscount;
        public TaxDiscountService(ITaxDiscount taxDiscount)
        {
            _taxDiscount = taxDiscount;
        }

        public async Task<Tax> TaxAndDiscount(TaxDiscountDTO dto)
        {
            return await _taxDiscount.TaxDiscountAsync(dto);
        }

        public async Task<string> UpdateTaxDiscount(UpdateTaxDiscountDTO dto)
        {
            return await _taxDiscount.UpdateTaxDiscount(dto);
        }

        public async Task<ICollection> GetAllTaxAndDiscount()
        {
            return await _taxDiscount.GetAllTaxAndDiscount();
        }

        public async Task<string> DeleteTaxAndDiscount(int id)
        {
            return await _taxDiscount.DeleteTaxAndDiscount(id);
        }


    }
}
