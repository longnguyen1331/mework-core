namespace Contract.AppConfigs
{
    public interface IAppConfigService
    {
        Task<ApiResponseBase<List<AppConfigDto>>> GetListAsync();
        Task<AppConfigDto> GetAppliedConfigAsync();
        Task<AppConfigDto> CreateAsync(CreateUpdateAppConfigDto input);
        Task<AppConfigDto> UpdateAsync(CreateUpdateAppConfigDto input,Guid id);
        Task DeleteAsync(Guid id);
        Task ApplyConfig(Guid id);
        Task SwitchOffConfig(Guid id);
        Task<AppConfigDto> AppconfigActiveAsync();
    }
}