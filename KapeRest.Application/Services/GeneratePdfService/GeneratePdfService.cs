using KapeRest.Application.Interfaces.GeneratePdf;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Infrastructure.Services.GeneratePdfService
{
    public class GeneratePdfService : ISalesPdf
    {
        public byte[] GeneratePdf<T>(T document)
        {
            if (document is IDocument questDocument)
            {
                return questDocument.GeneratePdf();
            }

            throw new ArgumentException("Document type not supported");
        }
    }
}
