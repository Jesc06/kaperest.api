using KapeRest.Application.DTOs.Admin.Branch;
using KapeRest.Core.Entities.Branch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.Admin.Branch
{
    public interface IBranch
    {
        Task<BranchDTO> AddBranch(BranchDTO dto, string userId, string role);
        Task<object> GetBranches();
        Task<string> DeleteBranch(int id, string userId, string role);
        Task<BranchDTO> UpdateBranch(BranchDTO dto, string userId, string role);
    }
}
