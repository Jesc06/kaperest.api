using KapeRest.Application.Interfaces.Admin.Audit;
using KapeRest.Application.Interfaces.Admin.Branch;
using KapeRest.Application.Interfaces.Admin.CreateMenuItem;
using KapeRest.Application.Interfaces.Admin.Inventory;
using KapeRest.Application.Interfaces.Admin.PendingAcc;
using KapeRest.Application.Interfaces.Admin.Supplier;
using KapeRest.Application.Interfaces.Auth;
using KapeRest.Application.Interfaces.Cashiers.Buy;
using KapeRest.Application.Interfaces.Cashiers.Sales;
using KapeRest.Application.Interfaces.CurrentUserService;
using KapeRest.Application.Interfaces.PayMongo;
using KapeRest.Application.Interfaces.PdfService;
using KapeRest.Application.Services.Admin.Audit;
using KapeRest.Application.Services.Admin.Branch;
using KapeRest.Application.Services.Admin.CreateMenuItem;
using KapeRest.Application.Services.Admin.Inventory;
using KapeRest.Application.Services.Admin.PendingAcc;
using KapeRest.Application.Services.Admin.Supplier;
using KapeRest.Application.Services.Auth;
using KapeRest.Application.Services.Cashiers.Buy;
using KapeRest.Application.Services.Cashiers.Sales;
using KapeRest.Application.UseCases.Sales;
using KapeRest.Infrastructure.Persistence.Repositories.Admin.Audit;
using KapeRest.Infrastructure.Persistence.Repositories.Admin.Branch;
using KapeRest.Infrastructure.Persistence.Repositories.Cashiers.Sales;
using KapeRest.Infrastructure.Services.PayMongoService;
using KapeRest.Infrastructure.Services.PdfServices;
using KapeRest.Infrastructures.Persistence.Repositories.Account;
using KapeRest.Infrastructures.Persistence.Repositories.Admin.CreateMenuItem;
using KapeRest.Infrastructures.Persistence.Repositories.Admin.Inventory;
using KapeRest.Infrastructures.Persistence.Repositories.Admin.PendingAccounts;
using KapeRest.Infrastructures.Persistence.Repositories.Admin.Suppliers;
using KapeRest.Infrastructures.Persistence.Repositories.Cashiers.Buy;
using KapeRest.Infrastructures.Services.CurrentUserService;
using KapeRest.Infrastructures.Services.JwtService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Paymongo.Sharp;
using Paymongo.Sharp.Features.Payments;
using Paymongo.Sharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace KapeRest.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSevices(this IServiceCollection services)
        {
            #region --Dependency Injection--
            services.AddScoped<IAccounts, RegisterAccountRepositories>();
            services.AddScoped<AccountService>();

            services.AddScoped<IJwtService, GenerateTokenService>();

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUser, CurrentUserService>();

            services.AddScoped<IpendingAccount, PendingAccountRepository>();
            services.AddScoped<PendingAccService>();

            services.AddScoped<IInventory, AddProductRepository>();
            services.AddScoped<AddProductService>();

            services.AddScoped<ISupplier, AddSupplierRepository>();
            services.AddScoped<AddSupplierService>();

            services.AddScoped<IMenuItem, MenuItemRepository>();
            services.AddScoped<MenuItemService>();

            services.AddScoped<IBuy, BuyRepository>();
            services.AddScoped<BuyService>();

      
            services.AddScoped<IBranch, BranchRepository>();
            services.AddScoped<BranchService>();

            services.AddScoped<ICashierSalesReport, CashierSalesReportRepository>();
            services.AddScoped<CashierSalesReportService>();

            //pdf
            services.AddScoped<IPdfService, PdfService>();
            services.AddScoped<GenerateSalesReportUseCase>();

            services.AddScoped<IAdminSalesReport, AdminSalesReportsRepository>();
            services.AddScoped<AdminSalesReportService>();

            services.AddScoped<IOverallSales, OverAllSalesRepository>();
            services.AddScoped<OverAllSalesService>();

            services.AddScoped<IStaffSalesReport, StaffSalesReportRepository>();
            services.AddScoped<StaffSalesService>();

            services.AddScoped<IsalesTransaction, SalesTransactionRepositories>();
            services.AddScoped<SalesTransactionService>();

            services.AddScoped<IAudit, AuditRepository>();
            services.AddScoped<AuditService>();


            #endregion


            return services;
        }
    }
}
