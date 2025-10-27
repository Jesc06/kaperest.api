using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.DTOs.Admin.TaxDiscount
{
    public class UpdateTaxDiscountDTO
    {
        public int Id { get; set; }
        public string SettingName { get; set; }
        public decimal Value { get; set; }
        public string Description { get; set; }
    }
}
