using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using KapeRest.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;

namespace KapeRest.Infrastructure.Persistence.Seeder
{
    public class AdminSeededAccount
    {
        public static async Task AdminAccount(RoleManager<IdentityRole> roleManager, 
                                              UserManager<UsersIdentity> userManager, IConfiguration configuration)
        {
            string[] roles = { "Admin", "Staff", "Cashier" };

            foreach(var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
            }

            var AdminEmail = Environment.GetEnvironmentVariable("Admin_Email")!;
            var AdminPassword = Environment.GetEnvironmentVariable("Admin_Password")!;
            var FirstName = Environment.GetEnvironmentVariable("Admin_FirstName")!;
            var MiddleName = Environment.GetEnvironmentVariable("Admin_MiddleName")!;
            var LastName = Environment.GetEnvironmentVariable("Admin_LastName")!;

            var findUser = await userManager.FindByEmailAsync(AdminEmail);  
            if(findUser is null)
            {
                var createUser = new UsersIdentity
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    FirstName = FirstName,
                    MiddleName = MiddleName,
                    LastName = LastName,
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
