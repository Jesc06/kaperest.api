using KapeRest.Application.DTOs.Admin.Branch;
using KapeRest.Application.Interfaces.Admin.Branch;
using KapeRest.Core.Entities.Branch;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Infrastructure.Persistence.Repositories.Admin.Branch
{
    public class BranchRepo : IBranch
    {
        private readonly ApplicationDbContext _context;
        public BranchRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BranchDTO> AddBranch(BranchDTO dto)
        {
            var exists = await _context.Branches.AnyAsync(x => x.BranchName == dto.Name);
            if (exists)
                throw new Exception("Branch already exists.");

            var branch = new BranchEntities
            {
                BranchName = dto.Name,
                Location = dto.Location
            };

            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<object> GetBranches()
        {
            var get = await _context.Branches
                .Include(b => b.PendingAccounts)
                .Select(b => new
                {
                    b.BranchName,
                    b.Location,
                    PendingAccounts = b.PendingAccounts.Select(p => new
                    {
                        p.FirstName,
                        p.MiddleName,
                        p.LastName,
                        p.Email,
                        p.Role
                    })
                }).ToListAsync();
            return get;
        }

        public async Task<string> DeleteBranch(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
                return "Branch not found.";

            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();
            return "Successfully deleted!";
        }

        public async Task<BranchDTO> UpdateBranch(BranchDTO dto)
        {
            var branch = await _context.Branches.FindAsync(dto.Id);
            if (branch == null)
                throw new Exception("Branch not found.");

            branch.BranchName = dto.Name;
            branch.Location = dto.Location;
            await _context.SaveChangesAsync();
            return dto;
        }


    }
}
