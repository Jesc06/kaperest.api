using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.Users.Sales
{
    public interface ISalesReports
    {
        byte[] GeneratePdf(string htmlContent);
    }
}
