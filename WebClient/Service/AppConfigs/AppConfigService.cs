using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contract;
using Contract.AppConfigs;
using Contract.AppHistories;
using WebClient.RequestHttp;

namespace WebClient.Service.AppConfigs
{
    public class AppConfigService : IAppConfigService
    {
                
        public async Task<ApiResponseBase<List<AppConfigDto>>> GetListAsync()
        {
            return await RequestClient.GetAPIAsync<ApiResponseBase<List<AppConfigDto>>>("app-config");

        }

        public async Task<AppConfigDto> GetAppliedConfigAsync()
        {
            return await RequestClient.GetAPIAsync<AppConfigDto>("app-config/get-applied-config");
        }

        public async Task<AppConfigDto> CreateAsync(CreateUpdateAppConfigDto input)
        {
            return await RequestClient.PostAPIAsync<AppConfigDto>("app-config",input);
        }

        public async  Task<AppConfigDto> UpdateAsync(CreateUpdateAppConfigDto input, Guid id)
        {
            return await RequestClient.PutAPIAsync<AppConfigDto>($"app-config/{id}",input);
        }

        public async Task DeleteAsync(Guid id)
        {
            await RequestClient.DeleteAPIAsync<Task>($"app-config/{id}");
        }

        public async Task ApplyConfig(Guid id)
        {
            await RequestClient.PatchAPIAsync<Task>($"app-config/apply-config/{id}",null);
        }

        public async Task SwitchOffConfig(Guid id)
        {
            await RequestClient.PatchAPIAsync<Task>($"app-config/switch-off-config/{id}",null);
        }

        public Task<AppConfigDto> AppconfigActiveAsync()
        {
            throw new NotImplementedException();
        }
    }
}