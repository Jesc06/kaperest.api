using KapeRest.Application.DTOs.Admin.Branch;
using KapeRest.Application.Interfaces.Admin.Branch;
using KapeRest.Application.Interfaces.CurrentUserService;
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
        private readonly ICurrentUser _currentUser;
        public BranchService(IBranch branch, ICurrentUser currentUser)
        {
            _branch = branch;
            _currentUser = currentUser; 
        }

        public async Task<BranchDTO> AddBranch(BranchDTO add)
        {
            var user = _currentUser.Email;
            var role = _currentUser.Role;
            return await _branch.AddBranch(add, user, role);
        }

        public async Task<string> DeleteBranch(int id)
        {
            var user = _currentUser.Email;
            var role = _currentUser.Role;
            return await _branch.DeleteBranch(id, user, role);
        }

        public async Task<BranchDTO> UpdateBranch(BranchDTO update)
        {
            var user = _currentUser.Email;
            var role = _currentUser.Role;
            return  await _branch.UpdateBranch(update, user, role);
        }
        public async Task<object>GetAllBranch()
        {
            return await _branch.GetBranches();
        }

    }
}
