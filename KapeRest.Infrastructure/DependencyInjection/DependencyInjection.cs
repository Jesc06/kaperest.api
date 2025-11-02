using KapeRest.Application.Interfaces.Admin.Branch;
using KapeRest.Application.Interfaces.Admin.CreateMenuItem;
using KapeRest.Application.Interfaces.Admin.Inventory;
using KapeRest.Application.Interfaces.Admin.PendingAcc;
using KapeRest.Application.Interfaces.Admin.Supplier;
using KapeRest.Application.Interfaces.Admin.TaxDiscount;
using KapeRest.Application.Interfaces.Auth;
using KapeRest.Application.Interfaces.CurrentUserService;
using KapeRest.Application.Interfaces.Jwt;
using KapeRest.Application.Interfaces.PdfService;
using KapeRest.Application.Interfaces.Users.Buy;
using KapeRest.Application.Interfaces.Users.Sales;
using KapeRest.Application.Services.Admin.Branch;
using KapeRest.Application.Services.Admin.CreateMenuItem;
using KapeRest.Application.Services.Admin.Inventory;
using KapeRest.Application.Services.Admin.PendingAcc;
using KapeRest.Application.Services.Admin.Supplier;
using KapeRest.Application.Services.Admin.TaxDiscount;
using KapeRest.Application.Services.Auth;
using KapeRest.Application.Services.Users.Buy;
using KapeRest.Application.Services.Users.Sales;
using KapeRest.Application.UseCases.Sales;
using KapeRest.Infrastructure.Persistence.Repositories.Admin.Branch;
using KapeRest.Infrastructure.Persistence.Repositories.Admin.TaxDiscount;
using KapeRest.Infrastructure.Persistence.Repositories.Users.Sales;
using KapeRest.Infrastructure.Services.PdfServices;
using KapeRest.Infrastructures.Persistence.Repositories.Account;
using KapeRest.Infrastructures.Persistence.Repositories.Admin.CreateMenuItem;
using KapeRest.Infrastructures.Persistence.Repositories.Admin.Inventory;
using KapeRest.Infrastructures.Persistence.Repositories.Admin.PendingAccounts;
using KapeRest.Infrastructures.Persistence.Repositories.Admin.Suppliers;
using KapeRest.Infrastructures.Persistence.Repositories.Users.Buy;
using KapeRest.Infrastructures.Services.CurrentUserService;
using KapeRest.Infrastructures.Services.JwtService;
using Microsoft.Extensions.DependencyInjection;
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

            services.AddScoped<IpendingAccount, PendingAccountRepo>();
            services.AddScoped<PendingAccService>();

            services.AddScoped<IInventory, AddProductRepo>();
            services.AddScoped<AddProductService>();

            services.AddScoped<ISupplier, AddSupplierRepo>();
            services.AddScoped<AddSupplierService>();

            services.AddScoped<IMenuItem, MenuItemRepo>();
            services.AddScoped<MenuItemService>();

            services.AddScoped<IBuy, BuyRepo>();
            services.AddScoped<BuyService>();

            services.AddScoped<ITaxDiscount, TaxDiscountRepo>();
            services.AddScoped<TaxDiscountService>();

            services.AddScoped<IBranch, BranchRepo>();
            services.AddScoped<BranchService>();

            services.AddScoped<ISales, SalesRepo>();
            services.AddScoped<SalesService>();

            //pdf
            services.AddScoped<IPdfService, PdfService>();
            
            services.AddScoped<GenerateSalesReportUseCase>();

            #endregion
            return services;
        }
    }
}
