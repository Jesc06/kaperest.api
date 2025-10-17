using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Application.Interfaces.Account;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using KapeRest.Application.DTOs.Account;

namespace KapeRest.Infrastructures.Persistence.Repositories.Account
{
    public class RegisterAccountRepositories : IAccounts
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly ApplicationDbContext _context;
        public RegisterAccountRepositories(
                UserManager<Users> userManager,
                SignInManager<Users> signInManager,
                ApplicationDbContext context
                )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<bool> RegisterAccount(RegisterAccountDTO register)
        {
            var users = new Users
            {
                FirstName = register.FirstName,
                MiddleName = register.MiddleName,
                LastName = register.LastName,
                UserName = register.Email,
                Email = register.Email,
            };
            var registerUser = await _userManager.CreateAsync(users, register.Password);
            if (registerUser.Succeeded)
            {
                await _userManager.AddToRoleAsync(users, register.Roles);
                return true;
            }
            return false;
        }

        public async Task<bool> Login(LoginDTO login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user == null)
            {
                Console.WriteLine("User not found");
                return false;
            }

            var result = await _signInManager.PasswordSignInAsync(user, login.Password, false, false);
            Console.WriteLine($"Login succeeded: {result.Succeeded}, LockedOut: {result.IsLockedOut}, NotAllowed: {result.IsNotAllowed}");
            return result.Succeeded;
        }




    }
}
