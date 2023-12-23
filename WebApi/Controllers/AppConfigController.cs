using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.AppConfigs;
using Contract;
using Contract.AppConfigs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    
    [ApiController]
    [Route("api/app-config/")]
    [Authorize]
    public class AppConfigController : IAppConfigService
    {
        private readonly AppConfigService _appConfigService;
        public AppConfigController(AppConfigService appConfigService)
        {
            _appConfigService = appConfigService;
        }
        [HttpGet]
        public async Task<ApiResponseBase<List<AppConfigDto>>> GetListAsync()
        {
            return await _appConfigService.GetListAsync();
        }

        [HttpGet]
        [Route("get-applied-config")]
        public async Task<AppConfigDto> GetAppliedConfigAsync()
        {
            return await _appConfigService.GetAppliedConfigAsync();
        }

        [HttpPost]
        public async Task<AppConfigDto> CreateAsync(CreateUpdateAppConfigDto input)
        {
            return await _appConfigService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<AppConfigDto> UpdateAsync(CreateUpdateAppConfigDto input, Guid id)
        {
            return await _appConfigService.UpdateAsync(input,id);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
             await _appConfigService.DeleteAsync(id);
        }

        [HttpPatch]
        [Route("apply-config/{id}")]
        public async Task ApplyConfig(Guid id)
        {
            await _appConfigService.ApplyConfig(id);
        }

        [HttpPatch]
        [Route("switch-off-config/{id}")]
        public async Task SwitchOffConfig(Guid id)
        {
            await _appConfigService.SwitchOffConfig(id);
        }
        [HttpGet]
        [Route("using-config")]
        [AllowAnonymous]
        public async Task<AppConfigDto> AppconfigActiveAsync()
        {
            return await _appConfigService.AppconfigActiveAsync();
        }
    }
}