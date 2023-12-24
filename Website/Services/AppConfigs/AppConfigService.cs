using Contract.AppConfigs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Website.Models;

namespace Website.Services.AppConfigs
{
    public class AppConfigService : IAppConfigService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly RemoteAPIConfig _remoteOptions;
        private string CacheName = "AppsettingCache";

        public AppConfigService(IHttpClientFactory httpClientFactory,
                                IOptions<RemoteAPIConfig> remoteOptions,
                                IMemoryCache memoryCache)
        {
            if (remoteOptions is null)
            {
                throw new ArgumentNullException(nameof(remoteOptions));
            }

            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _remoteOptions = remoteOptions.Value;
        }

        public async Task<AppConfigDto?> GetCurrentAppConfigAsync()
        {
            _memoryCache.TryGetValue(CacheName, out AppConfigDto? cacheData);

            if (cacheData == null)
            {
                var client = _httpClientFactory.CreateClient("Website");
                var response = await client.GetStringAsync(_remoteOptions.GetAppConfig);

                cacheData = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<AppConfigDto>(response) : null;

                _memoryCache.Set(CacheName, cacheData);
            }
            
            return cacheData;
        }
    }
}
