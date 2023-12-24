using Contract.AppConfigs;

namespace Website.Services.AppConfigs
{
    public interface IAppConfigService
    {
        Task<AppConfigDto?> GetCurrentAppConfigAsync();
    }
}
