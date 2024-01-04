using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Identity.RoleManager;
using Contract;
using Contract.Identity.RoleManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/role/")]
    [Authorize]
    public class RoleController(RoleManagerService _roleManagerService)
    {
        [HttpGet]
        public async Task<List<RoleDto>> GetListAsync()
        {
            return await _roleManagerService.GetListAsync();
        }

        [HttpPost]
        public async Task<RoleDto> CreateAsync(CreateUpdateRoleDto input)
        {
            return await _roleManagerService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<RoleDto> UpdateAsync(CreateUpdateRoleDto input, Guid id)
        {
            return await _roleManagerService.UpdateAsync(input, id);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
             await _roleManagerService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("get-claims/{roleId}")]
        public async Task<List<RoleClaimDto>> GetClaimListAsync(Guid roleId)
        {
            return await _roleManagerService.GetClaimListAsync(roleId);
        }

        [HttpPost]
        [Route("create-claim")]
        [Authorize("EmployeeOnly")]
        public async Task<CreateUpdateClaimRole> CreateClaimAsync(CreateUpdateClaimRole input)
        {
            return await _roleManagerService.CreateClaimAsync(input);
        }

 

        [HttpPost]
        [Route("delete-claim")]
        public async Task DeleteClaimAsync(RoleClaimModel input)
        {
             await _roleManagerService.DeleteClaimAsync(input);
        }
    }
}