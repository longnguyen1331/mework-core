namespace Contract.BackupDetails
{
    public interface IBackupDetailService
    {
        Task<ApiResponseBase<BackupDetailsearchResponseDto>> GetListAsync(BackupDetailFilterPagingDto filter);
        Task<ApiResponseBase<bool>> CreateAsync(CreateUpdateBackupDetailDto input);
    }
}