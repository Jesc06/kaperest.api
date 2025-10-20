using KapeRest.Application.DTOs.Admin.PendingAccount;
using KapeRest.Application.Interfaces.Admin.PendingAcc;
using KapeRest.Domain.Entities.PendingAccounts;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KapeRest.Infrastructures.Persistence.Repositories.Admin.PendingAccounts
{
    public class PendingAccountRepo : IpendingAccount
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Users> _userManager;
        public PendingAccountRepo(ApplicationDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task RegisterPending(PendingAccDTO pending)
        {
            var alreadyExists = await _context.PendingUserAccount.FirstOrDefaultAsync(x => x.Email == pending.Email);
                if(alreadyExists is not null)
                    throw new Exception("A pending account with this email already exists.");

                var pendingUser = new PendingUserAccount
                {
                    FirstName = pending.FirstName,
                    MiddleName = pending.MiddleName,
                    LastName = pending.LastName,
                    Email = pending.Email,
                    Password = pending.Password,
                    Role = pending.Role,
                    Status = "Pending"
                };

            _context.PendingUserAccount.Add(pendingUser);
            await _context.SaveChangesAsync();  
        }

        public async Task ApprovePendingAccount(int id)
        {
            var pending = await _context.PendingUserAccount.FindAsync(id);
            if (pending == null)
                throw new Exception("Pending account not found.");
            
            if(pending.Status is not "Pending")
                throw new Exception("Account already proceed");

            var user = new Users
            {
                FirstName = pending.FirstName,
                MiddleName = pending.MiddleName,
                LastName = pending.LastName,
                UserName = pending.Email,
                Email = pending.Email,
            };

            var result = await _userManager.CreateAsync(user, pending.Password);
            if(!result.Succeeded)
                throw new Exception("Failed to create user account.");

            await _userManager.AddToRoleAsync(user, pending.Role);
            pending.Status = "Approved";
            await _context.SaveChangesAsync();  

        }

        public async Task RejectPendingAccount(int id)
        {
            var pending = await _context.PendingUserAccount.FindAsync(id);
            if (pending == null)
                throw new Exception("Pending account not found.");

            _context.PendingUserAccount.Remove(pending);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PendingUserAccount>> GetPendingAccounts()
        {
            var list = await _context.PendingUserAccount.ToListAsync();
            return list;
        }


    }
}
