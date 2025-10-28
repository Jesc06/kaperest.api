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

        public async Task<BranchDTO> AddBranch(BranchDTO add)
        {
            return await _branch.AddBranch(add);
        }

        public async Task<string> DeleteBranch(int id)
        {
            return await _branch.DeleteBranch(id);
        }

        public async Task<BranchDTO> UpdateBranch(BranchDTO update)
        {
           return  await _branch.UpdateBranch(update);
        }
        public async Task<object>GetAllBranch()
        {
            return await _branch.GetBranches();
        }

    }
}
