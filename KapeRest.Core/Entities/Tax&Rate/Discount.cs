using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Core.Entities.Tax_Rate
{
    public class Discount
    {
        public int Id { get; set; }
        public string TaxRate { get; set; }
        public decimal Value { get; set; }
        public string Description { get; set; }
    }
}
