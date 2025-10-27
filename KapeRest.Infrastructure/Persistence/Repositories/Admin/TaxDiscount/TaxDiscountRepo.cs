using KapeRest.Application.DTOs.Admin.TaxDiscount;
using KapeRest.Application.Interfaces.Admin.TaxDiscount;
using KapeRest.Core.Entities.Tax_Rate;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Infrastructure.Persistence.Repositories.Admin.TaxDiscount
{
    public class TaxDiscountRepo : ITaxDiscount
    {
        private readonly ApplicationDbContext _context;
        public TaxDiscountRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<TaxAndRate> TaxDiscountAsync(TaxDiscountDTO dto)
        {
            var rate = new TaxAndRate
            {
                SettingName = dto.SettingName,
                Value = dto.Value,
                Description = dto.Description
            };
            _context.TaxAndRate.Add(rate);
            await _context.SaveChangesAsync();
            return rate;
        }

        public async Task<string> UpdateTaxDiscount(UpdateTaxDiscountDTO dto)
        {
            var taxDiscount = await _context.TaxAndRate.FirstOrDefaultAsync(t => t.Id == dto.Id);
            if(taxDiscount is not null)
            {
                taxDiscount.SettingName = dto.SettingName;
                taxDiscount.Value = dto.Value;
                taxDiscount.Description = dto.Description;
                await _context.SaveChangesAsync();
                return "Successfully Updated";
            }
            return "failed to update";
        }

    }
}
