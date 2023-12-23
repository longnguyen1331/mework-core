using Contract;
using Contract.AppHistories;
using WebClient.RequestHttp;

namespace WebClient.Service.AppHistories
{
    public class AppHistoryService
    {
        public async Task<ApiResponseBase<AppHistorySearchResponseDto>> GetListAsync(AppHistoryFilterPagingDto filter)
        {
            return await RequestClient.PostAPIAsync<ApiResponseBase<AppHistorySearchResponseDto>>("appHistories/search", filter);
        }
    }
}
