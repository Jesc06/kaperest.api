using KapeRest.Application.DTOs.Admin.Branch;
using KapeRest.Application.Interfaces.Admin.Branch;
using KapeRest.Core.Entities.Branch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Services.Admin.Branch
{
    public class BranchService
    {
        private readonly IBranch _branch;
        public BranchService(IBranch branch)
        {
            _branch = branch;
        }

        public async Task<BranchDTO> AddBranch(BranchDTO add, string userId, string role)
        {
            return await _branch.AddBranch(add, userId, role);
        }

        public async Task<string> DeleteBranch(int id, string userId, string role)
        {
            return await _branch.DeleteBranch(id, userId, role);
        }

        public async Task<BranchDTO> UpdateBranch(BranchDTO update, string userId, string role)
        {
           return  await _branch.UpdateBranch(update, userId, role);
        }
        public async Task<object>GetAllBranch()
        {
            return await _branch.GetBranches();
        }

    }
}
