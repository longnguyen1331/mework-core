namespace Contract.AppHistories
{
    public interface IAppHistoryService
    {
        Task<ApiResponseBase<List<AppHistoryDto>>> GetListAsync();
        Task CreateAsync(CreateUpdateAppHistoryDto input);
        Task<AppHistoryDto> UpdateAsync(CreateUpdateAppHistoryDto input,Guid id);
    }
}