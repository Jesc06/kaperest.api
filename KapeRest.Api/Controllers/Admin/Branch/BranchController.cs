using KapeRest.Application.DTOs.Admin.Branch;
using KapeRest.Application.Services.Admin.Branch;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KapeRest.Api.Controllers.Admin.Branch
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly BranchService _branchService;
        public BranchController(BranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpPost("AddBranch")]
        public async Task<ActionResult> AddBranch(BranchDTO add)
        {
          var result = await _branchService.AddBranch(add);
          return Ok(result);
        }

        [HttpPut("UpdateBranch")]
        public async Task<ActionResult> UpdateBranch(BranchDTO add)
        {
            var userIdFromJwt = User.FindFirst("sub")?.Value;
            var roleFromJwt = User.FindFirst("role")?.Value ?? "Admin";

            var result = await _branchService.UpdateBranch(add);
            return Ok(result);
        }

        [HttpDelete("DeleteBranch")]
        public async Task<ActionResult>DeleteBranch(int id)
        {
            var result = await _branchService.DeleteBranch(id);
            return Ok(result);
        }

        [HttpGet("GetAllBranch")]
        public async Task<ActionResult> GetAllBranch()
        {
            var result = await _branchService.GetAllBranch();
            return Ok(result);
        }


    }
}
