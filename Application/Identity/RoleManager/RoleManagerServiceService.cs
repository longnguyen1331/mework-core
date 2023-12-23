using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Contract;
using Contract.Identity.RoleManager;
using Core.Const;
using Core.Exceptions;
using Domain.Identity.RoleClaims;
using Domain.Identity.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SqlServ4r.Repository.RoleClaims;
using Volo.Abp.DependencyInjection;

namespace Application.Identity.RoleManager
{
    public class RoleManagerService : ServiceBase, IRoleManagerService,ITransientDependency
    {
        private RoleManager<Role> _roleManager;
        private RoleClaimRepository _roleClaimRepository;

        public RoleManagerService(RoleManager<Role> roleManager,
            RoleClaimRepository roleClaimRepository)
        {
            _roleManager = roleManager;
            _roleClaimRepository = roleClaimRepository;
        }

   
        public async Task<List<RoleDto>> GetListAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var rolesDto = ObjectMapper.Map<List<Role>, List<RoleDto>>(roles);
            return rolesDto;
        }

        public async Task<RoleDto> CreateAsync(CreateUpdateRoleDto input)
        {
            
            input.Name = input.Name.Trim().ToUpper();
            input.Code = input.Code?.Trim().ToUpper();
            var role = ObjectMapper.Map<CreateUpdateRoleDto, Role>(input);
            
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                throw new GlobalException(result.Errors?.FirstOrDefault().Description, HttpStatusCode.BadRequest);

            }

            return ObjectMapper.Map<Role,RoleDto>(role);
        }

        public async Task<RoleDto> UpdateAsync(CreateUpdateRoleDto input, Guid id)
        {
            input.Name = input.Name.Trim().ToUpper();
            input.Code = input.Code?.Trim().ToUpper();
            var item = await _roleManager.FindByIdAsync(id.ToString());

            if (item == null)
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }
            
            var role = ObjectMapper.Map(input, item);
            var result = await _roleManager.UpdateAsync(role);
            
            if (!result.Succeeded)
            {
                throw new GlobalException(result.Errors?.FirstOrDefault().Description, HttpStatusCode.BadRequest);

            }
            
            return  ObjectMapper.Map<Role,RoleDto>(role);
        }

        public async Task DeleteAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }

            var result = await _roleManager.DeleteAsync(role);
            
            if (!result.Succeeded)
            {
                throw new GlobalException(result.Errors?.FirstOrDefault().Description, HttpStatusCode.BadRequest);

            }
        }

        public async Task<RoleDto> CreateWithClaimsAsync(CreateUpdateRoleDto input)
        {
            var role = await CreateAsync(input);
            var claims = ObjectMapper.Map<List<CreateUpdateClaimRole>,List<RoleClaim>>(input.Claims);
            _roleClaimRepository.GetQueryable().AddRange(claims);
            return role;
        }

        public async Task<RoleDto> UpdateWithClaimsAsync(CreateUpdateRoleDto input, Guid id)
        {
           await  UpdateAsync(input, id);
           await DeleteClaimByRoleIDAsync(id);
           return null;
        }

        public async Task<List<RoleClaimDto>> GetClaimListAsync(Guid roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)   throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            var claims = await _roleManager.GetClaimsAsync(role);
            List<RoleClaimDto> roleClaim = new List<RoleClaimDto>();

            foreach (var item in claims)
            {
                roleClaim.Add(new RoleClaimDto(){RoleId = roleId , ClaimType = item.Type,ClaimValue = item.Value});
            }
            
            return roleClaim;
        }

        
        public async Task<CreateUpdateClaimRole> CreateClaimAsync(CreateUpdateClaimRole input)
        {
            var role = await _roleManager.FindByIdAsync(input.RoleId.ToString());
            if (role == null) throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            
            var result =  await _roleManager.AddClaimAsync(role, new Claim(input.ClaimType, input.ClaimValue));
            return input;
        }

        public Task<CreateUpdateClaimRole> CreateClaimsAsync(List<CreateUpdateClaimRole> inputs)
        {
            throw new NotImplementedException();
        }


        public async Task DeleteClaimAsync(RoleClaimModel input)
        {
            var role = await _roleManager.FindByIdAsync(input.RoleId.ToString());
            if (role == null) throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            var result = await _roleManager.RemoveClaimAsync(role, new Claim(input.ClaimType,input.ClaimValue));
            
            if (!result.Succeeded)
            {
                throw new GlobalException(result.Errors?.FirstOrDefault().Description, HttpStatusCode.BadRequest);
            }
            
        }

        public async Task DeleteClaimByRoleIDAsync(Guid roleId)
        {
           var claims = await _roleClaimRepository.GetListAsync(x => x.RoleId == roleId);
           _roleClaimRepository.RemoveRange(claims);
        }
    }
}