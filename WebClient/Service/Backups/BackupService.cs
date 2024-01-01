using Contract;
using Contract.Backups;
using WebClient.RequestHttp;

namespace WebClient.Service.Backups
{
    public class BackupService : IBackupService
    {
        public async Task<ApiResponseBase<BackupSearchResponseDto>> GetListAsync(BackupFilterPagingDto filter)
        {
            return await RequestClient.PostAPIAsync<ApiResponseBase<BackupSearchResponseDto>>("backups/search", filter);
        }
        public async Task<ApiResponseBase<bool>> TestConnectionAsync(Guid id)
        {
            return await RequestClient.GetAPIAsync<ApiResponseBase<bool>>($"backups/testConnection/{id}");
        }
        public async Task<ApiResponseBase<bool>> CreateAsync(CreateUpdateBackupDto input)
        {
            return await RequestClient.PostAPIAsync<ApiResponseBase<bool>>("backups", input);
        }
        public async Task<ApiResponseBase<bool>> DeleteAsync(Guid id)
        {
            return await RequestClient.DeleteAPIAsync<ApiResponseBase<bool>>($"backups/{id}");
        }

        public async Task<ApiResponseBase<BackupDto>> UpdateAsync(CreateUpdateBackupDto input, Guid id)
        {
            return await RequestClient.PutAPIAsync<ApiResponseBase<BackupDto>>($"backups/{id}", input);
        }

    }
}
