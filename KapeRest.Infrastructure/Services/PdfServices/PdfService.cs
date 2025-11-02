using KapeRest.Application.DTOs.Users.Sales;
using KapeRest.Application.Interfaces.PdfService;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace KapeRest.Infrastructure.Services.PdfServices
{
    public class PdfService : IPdfService
    {
        public PdfService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateSalesReport(IEnumerable<SalesReportDTO> sales, string logopath, string roles)
        {
            var totalSales = sales.Sum(s => s.Total);
            var totalTransactions = sales.Count();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(40);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.SegoeUI));
                    #region--header--
                    page.Header().Row(row =>
                    {
                        if (System.IO.File.Exists(logopath))
                            row.ConstantItem(70).Height(70).Image(logopath, ImageScaling.FitArea);

                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("KapeRest")
                                .FontSize(22).SemiBold().FontColor(Colors.Brown.Medium);

                            column.Item().Text("Official Sales Report")
                                .FontSize(14).FontColor(Colors.Grey.Darken1);

                            column.Item().Text($"{DateTime.Now:MMMM dd, yyyy hh:mm tt}")
                                .FontSize(10).FontColor(Colors.Grey.Darken2);
                        });
                    });
                    #endregion--End header--
                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        column.Item().AlignCenter().Text($"Sales Report by {roles}")
                            .FontSize(18).SemiBold().FontColor(Colors.Brown.Medium);
                        column.Item().PaddingBottom(10);

                        #region--Table--
                        column.Item().Border(1).BorderColor(Colors.Grey.Lighten2)
                            .Padding(10)
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30);
                                    columns.RelativeColumn(1.2f);
                                    columns.RelativeColumn(1.2f);
                                    columns.RelativeColumn(1.2f);
                                    columns.RelativeColumn(1.2f);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                });
                                // Header Styling
                                table.Header(header =>
                                {
                                    static IContainer HeaderStyle(IContainer container) =>
                                        container.Background(Colors.Brown.Medium)
                                                 .Padding(5)
                                                 .DefaultTextStyle(x => x.FontColor(Colors.White).SemiBold());

                                    header.Cell().Element(HeaderStyle).Text("#");
                                    header.Cell().Element(HeaderStyle).Text("Cashier");
                                    header.Cell().Element(HeaderStyle).Text("Branch");
                                    header.Cell().Element(HeaderStyle).Text("Receipt No.");
                                    header.Cell().Element(HeaderStyle).Text("Date");
                                    header.Cell().Element(HeaderStyle).Text("Total");
                                    header.Cell().Element(HeaderStyle).Text("Status");
                                });
                                // Rows
                                int index = 1;
                                foreach (var s in sales)
                                {
                                    static IContainer CellStyle(IContainer container) =>
                                        container.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                                 .PaddingVertical(5).PaddingHorizontal(2);

                                    table.Cell().Element(CellStyle).Text(index++.ToString());
                                    table.Cell().Element(CellStyle).Text(s.CashierName);
                                    table.Cell().Element(CellStyle).Text(s.BranchName);
                                    table.Cell().Element(CellStyle).Text(s.ReceiptNumber);
                                    table.Cell().Element(CellStyle).Text(s.DateTime.ToString("MMM dd, yyyy"));
                                    table.Cell().Element(CellStyle).Text($"{s.Total.ToString("C", new CultureInfo("en-PH"))}");//Ph currency format
                                    table.Cell().Element(CellStyle).Text(s.Status);
                                }
                            });
                        #endregion--End Table--

                        #region--Overall Summary--
                        column.Item().PaddingTop(15)
                            .Row(row =>
                            {
                                row.RelativeItem().AlignLeft()
                                    .Text($"Total Transactions: {totalTransactions}")
                                    .FontColor(Colors.Grey.Darken1);

                                row.RelativeItem().AlignRight()
                                    .Text($"Total Sales: {totalSales.ToString("C", new CultureInfo("en-PH"))}")//Ph currency format
                                    .FontSize(13).SemiBold()
                                    .FontColor(Colors.Brown.Medium);
                            });
                        #endregion--End Overall Summary--
                    });
                    page.Footer()
                        .AlignCenter()
                        .Text("© 2025 KapeRest Café — All Rights Reserved")
                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                });
            });

            return document.GeneratePdf();
        }
    }
}
