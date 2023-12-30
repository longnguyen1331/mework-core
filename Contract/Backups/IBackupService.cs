namespace Contract.Backups
{
    public interface IBackupService
    {
        Task<ApiResponseBase<BackupSearchResponseDto>> GetListAsync(BackupFilterPagingDto filter);
        Task<ApiResponseBase<bool>> CreateAsync(CreateUpdateBackupDto input);
        Task<ApiResponseBase<bool>> DeleteAsync(Guid id);
        Task<ApiResponseBase<BackupDto>> UpdateAsync(CreateUpdateBackupDto input,Guid id);
    }
}