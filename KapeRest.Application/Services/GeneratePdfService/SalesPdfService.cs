using KapeRest.Application.DTOs.Users.Sales;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Infrastructure.Services.GeneratePdf
{
    public class SalesPdfService : IDocument
    {
        private readonly string _cashierName;
        private readonly IEnumerable<dynamic> _salesData;
        public SalesPdfService(string cashierName, IEnumerable<dynamic> salesData)
        {
            _cashierName = cashierName;
            _salesData = salesData;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Size(PageSizes.A4);

                page.Header().Text($"Sales Report - {_cashierName}")
                    .FontSize(20)
                    .Bold();

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Receipt #").Bold();
                        header.Cell().Text("Date").Bold();
                        header.Cell().Text("Subtotal").Bold();
                        header.Cell().Text("Tax").Bold();
                        header.Cell().Text("Discount").Bold();
                        header.Cell().Text("Total").Bold();
                        header.Cell().Text("Status").Bold();
                    });

                    foreach (var sale in _salesData)
                    {
                        table.Cell().Text($"{sale.ReceiptNumber}");
                        table.Cell().Text($"{sale.DateTime.ToString("MM/dd/yyyy HH:mm")}");
                        table.Cell().Text($"₱{sale.Subtotal:N2}");
                        table.Cell().Text($"₱{sale.Tax:N2}");
                        table.Cell().Text($"₱{sale.Discount:N2}");
                        table.Cell().Text($"₱{sale.Total:N2}");
                        table.Cell().Text($"{sale.Status}");
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        }


    }
}
