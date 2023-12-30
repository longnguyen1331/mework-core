using Contract;
using Contract.BackupDetails;
using WebClient.RequestHttp;

namespace WebClient.Service.BackupDetails
{
    public class BackupDetailService : IBackupDetailService
    {
        public async Task<ApiResponseBase<BackupDetailsearchResponseDto>> GetListAsync(BackupDetailFilterPagingDto filter)
        {
            return await RequestClient.PostAPIAsync<ApiResponseBase<BackupDetailsearchResponseDto>>("backupDetails/search", filter);
        }
        public async Task<ApiResponseBase<bool>> CreateAsync(CreateUpdateBackupDetailDto input)
        {
            return await RequestClient.PostAPIAsync<ApiResponseBase<bool>>("backupDetails", input);
        }
    }
}
