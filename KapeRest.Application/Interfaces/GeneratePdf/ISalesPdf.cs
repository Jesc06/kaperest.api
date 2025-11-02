using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.GeneratePdf
{
    public interface ISalesPdf
    {
        byte[] GeneratePdf<T>(T document);
    }
}
