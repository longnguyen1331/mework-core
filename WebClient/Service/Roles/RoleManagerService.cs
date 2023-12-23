using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contract;
using Contract.Identity.RoleManager;
using WebClient.RequestHttp;

namespace WebClient.Service.Roles
{
    public class RoleManagerService  : IRoleManagerService
    {
        public RoleManagerService()
        {
        }
        public async Task<List<RoleDto>> GetListAsync()
        {
           return await RequestClient.GetAPIAsync<List<RoleDto>>("role");
        }

        public async Task<RoleDto> CreateAsync(CreateUpdateRoleDto input)
        {
            return await RequestClient.PostAPIAsync<RoleDto>("role",input);

        }

        public async Task<RoleDto> UpdateAsync(CreateUpdateRoleDto input, Guid id)
        {
            return await RequestClient.PutAPIAsync<RoleDto>($"role/{id}" , input);

        }

        public async Task DeleteAsync(Guid id)
        { 
            await RequestClient.DeleteAPIAsync<Task>($"role/{id}");
        }

        public Task<List<RoleClaimDto>> GetClaimListAsync(Guid roleId)
        {
            throw new NotImplementedException();
        }

        public Task<CreateUpdateClaimRole> CreateClaimAsync(CreateUpdateClaimRole input)
        {
            throw new NotImplementedException();
        }

        public Task DeleteClaimAsync(RoleClaimModel input)
        {
            throw new NotImplementedException();
        }
    }
}