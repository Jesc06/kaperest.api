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

        public byte[] GenerateSalesReport(IEnumerable<SalesReportDTO> sales, string logoPath, string role)
        {
            var totalSales = sales.Sum(s => s.Total);
            var totalTransactions = sales.Count();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.SegoeUI));

                    //header
                    page.Header().Row(row =>
                    {
                        if (System.IO.File.Exists(logoPath))
                            row.ConstantItem(65).Height(65).Image(logoPath, ImageScaling.FitArea);

                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("KapeRest Café")
                                .FontSize(22).SemiBold().FontColor(Colors.Brown.Medium);

                            column.Item().Text("Official Sales Report")
                                .FontSize(13).FontColor(Colors.Grey.Darken1);

                            column.Item().Text($"{DateTime.Now:MMMM dd, yyyy hh:mm tt}")
                                .FontSize(10).FontColor(Colors.Grey.Darken2);
                        });
                    });

                    //Content
                    page.Content().PaddingVertical(15).Column(column =>
                    {
                        //Title
                        column.Item().AlignCenter().Text($"Sales Report by {role}")
                            .FontSize(20).SemiBold().FontColor(Colors.Brown.Medium);
                        column.Item().PaddingBottom(10);

                        //Table
                        column.Item().Table(table =>
                        {
                            
                            //Col
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(25);    // #
                                columns.RelativeColumn(1.1f);  // Username
                                columns.RelativeColumn(1.3f);  // Name
                                columns.RelativeColumn(1.4f);  // Email
                                columns.RelativeColumn(1.1f);  // Branch
                                columns.RelativeColumn(1.1f);  // BranchLocation
                                columns.RelativeColumn(1);     // Receipt No
                                columns.RelativeColumn(1.2f);  // Date
                                columns.RelativeColumn(0.8f);  // Subtotal
                                columns.RelativeColumn(0.8f);  // Tax
                                columns.RelativeColumn(0.8f);  // Discount
                                columns.RelativeColumn(1.1f);  // Total
                                columns.RelativeColumn(1f);    // Status
                            });

                            // Header
                            table.Header(header =>
                            {
                                static IContainer HeaderStyle(IContainer container) =>
                                    container.Background(Colors.Brown.Medium)
                                             .PaddingVertical(6)
                                             .PaddingHorizontal(4)
                                             .DefaultTextStyle(x => x.FontColor(Colors.White).SemiBold());

                                header.Cell().Element(HeaderStyle).Text("#");
                                header.Cell().Element(HeaderStyle).Text("Username");
                                header.Cell().Element(HeaderStyle).Text($"{role} Name");
                                header.Cell().Element(HeaderStyle).Text("Email");
                                header.Cell().Element(HeaderStyle).Text("Branch");
                                header.Cell().Element(HeaderStyle).Text("Branch Location");
                                header.Cell().Element(HeaderStyle).Text("Receipt No.");
                                header.Cell().Element(HeaderStyle).Text("Date");
                                header.Cell().Element(HeaderStyle).Text("Subtotal");
                                header.Cell().Element(HeaderStyle).Text("Tax");
                                header.Cell().Element(HeaderStyle).Text("Discount");
                                header.Cell().Element(HeaderStyle).Text("Total");
                                header.Cell().Element(HeaderStyle).Text("Status");
                            });

                            // Body Rows
                            int index = 1;
                            foreach (var s in sales)
                            {
                                static IContainer CellStyle(IContainer container) =>
                                    container.BorderBottom(0.5f)
                                             .BorderColor(Colors.Grey.Lighten2)
                                             .PaddingVertical(4)
                                             .PaddingHorizontal(3);

                                table.Cell().Element(CellStyle).Text(index++.ToString());
                                table.Cell().Element(CellStyle).Text(s.Username);
                                table.Cell().Element(CellStyle).Text(s.FullName);
                                table.Cell().Element(CellStyle).Text(s.Email);
                                table.Cell().Element(CellStyle).Text(s.BranchName);
                                table.Cell().Element(CellStyle).Text(s.BranchLocation);
                                table.Cell().Element(CellStyle).Text(s.MenuItemName);
                                table.Cell().Element(CellStyle).Text(s.DateTime.ToString("MMM dd, yyyy"));
                                table.Cell().Element(CellStyle).Text(s.Subtotal.ToString("C", new CultureInfo("en-PH")));
                                table.Cell().Element(CellStyle).Text(s.Tax.ToString("C", new CultureInfo("en-PH")));
                                table.Cell().Element(CellStyle).Text(s.Discount.ToString("C", new CultureInfo("en-PH")));
                                table.Cell().Element(CellStyle).Text(s.Total.ToString("C", new CultureInfo("en-PH")));
                                table.Cell().Element(CellStyle).Text(s.Status);
                            }
                        });

                        //Summary
                        column.Item().PaddingTop(15).Row(row =>
                        {
                            row.RelativeItem().AlignLeft()
                                .Text($"Total Transactions: {totalTransactions}")
                                .FontSize(11).FontColor(Colors.Grey.Darken1);

                            row.RelativeItem().AlignRight()
                                .Text($"Total Sales: {totalSales.ToString("C", new CultureInfo("en-PH"))}")
                                .FontSize(13).SemiBold().FontColor(Colors.Brown.Medium);
                        });
                    });

                    // Foooter
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("© 2025 KapeRest Café — All Rights Reserved")
                            .FontSize(9).FontColor(Colors.Grey.Darken1);
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
