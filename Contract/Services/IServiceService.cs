namespace Contract.Services
{
    public interface IServiceService
    {
        Task<ApiResponseBase<ServiceDto>> CreateAsync(CreateUpdateServiceDto input);
        Task<ApiResponseBase<ServiceDto>> UpdateAsync(CreateUpdateServiceDto input,Guid id);
        Task<ApiResponseBase<ServiceDto>> DeleteAsync(Guid id);
        Task<ApiResponseBase<ServiceDto>> GetByIdAsync(Guid id);
        Task<ApiResponseBase<List<ServiceDto>>> GetListAsync();
        Task<ApiResponseBase<List<ServiceDto>>> GetListPagingAsync(BaseFilterPagingDto filter);
        Task<ApiResponseBase<int>> IncreaseViewsTotalAsync(Guid id);
    }
}