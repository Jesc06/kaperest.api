using KapeRest.Application.DTOs.Admin.Branch;
using KapeRest.Application.Interfaces.Admin.Branch;
using KapeRest.Core.Entities.Branch;
using KapeRest.Domain.Entities.AuditLogEntities;
using KapeRest.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Infrastructure.Persistence.Database;

namespace KapeRest.Infrastructure.Persistence.Repositories.Admin.BranchRepo
{
    public class BranchRepository : IBranch
    {
        private readonly ApplicationDbContext _context;
        public BranchRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BranchDTO> AddBranch(BranchDTO dto, string userId, string role)
        {
            var exists = await _context.Branches.AnyAsync(x => x.BranchName == dto.Name);
            if (exists)
                throw new Exception("Branch already exists.");

            var branch = new BranchEntities
            {
                BranchName = dto.Name,
                Location = dto.Location,
                Staff = "N/A",
                Status = "N/A"
            };

            _context.Branches.Add(branch);

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = userId,
                Role = role,
                Action = "Added",
                Description = $"Added branch {dto.Name} at {dto.Location}",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<object> GetBranches()
        {
            var get = await _context.Branches
                .Select(b => new
                {
                    b.Id,
                    b.BranchName,
                    b.Location,
                    b.Staff,
                    b.Status
                }).ToListAsync();
            return get;
        }

        public async Task<string> DeleteBranch(int id, string userId, string role)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
                return "Branch not found.";

            _context.Branches.Remove(branch);

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = userId,
                Role = role,
                Action = "Deleted",
                Description = $"Deleted branch {branch.BranchName}",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return "Successfully deleted!";
        }

        public async Task<BranchDTO> UpdateBranch(BranchDTO dto, string userId, string role)
        {
            var branch = await _context.Branches.FindAsync(dto.Id);
            if (branch == null)
                throw new Exception("Branch not found.");

            branch.BranchName = dto.Name;
            branch.Location = dto.Location;
            branch.Staff = dto.Staff;
            branch.Status = dto.Status;

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = userId,
                Role = role,
                Action = "Updated",
                Description = $"Updated branch {dto.Name}",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return dto;
        }


    }
}
