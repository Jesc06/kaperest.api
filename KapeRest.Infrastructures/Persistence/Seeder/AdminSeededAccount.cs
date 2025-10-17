using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;

namespace KapeRest.Infrastructures.Persistence.Seeder
{
    public class AdminSeededAccount
    {
        public static async Task AdminAccount(RoleManager<IdentityRole> roleManager, 
                                              UserManager<Users> userManager, IConfiguration configuration)
        {
            string[] roles = { "Admin", "Staff", "Cashier" };

            foreach(var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
            }

            var AppSettingsJsonAdminAccount = configuration.GetSection("AdminSeededAccount");
            var AdminEmail = AppSettingsJsonAdminAccount["Email"];
            var AdminPassword = AppSettingsJsonAdminAccount["Password"];

            var findUser = await userManager.FindByEmailAsync(AdminEmail);  
            if(findUser is null)
            {
                var createUser = new Users
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    FirstName = AppSettingsJsonAdminAccount["FirstName"],
                    MiddleName = AppSettingsJsonAdminAccount["MiddleName"],
                    LastName = AppSettingsJsonAdminAccount["LastName"],
                    EmailConfirmed = true,
                };
                var createUserResult = await userManager.CreateAsync(createUser, AdminPassword);
                if (createUserResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(createUser, "Admin");
                }
            }
        }

    }
}
