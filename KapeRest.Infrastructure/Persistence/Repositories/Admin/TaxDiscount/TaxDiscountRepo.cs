using KapeRest.Application.DTOs.Admin.TaxDiscount;
using KapeRest.Application.Interfaces.Admin.TaxDiscount;
using KapeRest.Core.Entities.Tax_Rate;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections;
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
        public async Task<Tax> TaxDiscountAsync(TaxDiscountDTO dto)
        {
            var rate = new Tax
            {
                TaxRate = dto.SettingName,
                Value = dto.Value,
                Description = dto.Description
            };
            _context.Tax.Add(rate);
            await _context.SaveChangesAsync();
            return rate;
        }

        public async Task<string> UpdateTaxDiscount(UpdateTaxDiscountDTO dto)
        {
            var taxDiscount = await _context.Tax.FirstOrDefaultAsync(t => t.Id == dto.Id);
            if(taxDiscount is not null)
            {
                taxDiscount.TaxRate = dto.SettingName;
                taxDiscount.Value = dto.Value;
                taxDiscount.Description = dto.Description;
                await _context.SaveChangesAsync();
                return "Successfully Updated";
            }
            return "failed to update";
        }

        public async Task<ICollection> GetAllTaxAndDiscount()
        {
            var taxAndDiscounts = await _context.Tax.ToListAsync();
            return taxAndDiscounts;
        }

        public async Task<string> DeleteTaxAndDiscount(int id)
        {
            var taxAndDiscounts = await _context.Tax.FindAsync(id);
            if(taxAndDiscounts is null)
                return "Tax and Discount not found";

            _context.Tax.Remove(taxAndDiscounts);
            await _context.SaveChangesAsync();
            return "Successfully Deleted";
        }



    }
}
