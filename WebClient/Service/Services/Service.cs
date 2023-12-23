using Contract;
using Contract.Services;
using WebClient.RequestHttp;

namespace WebClient.Service.Services
{
    public class ServiceService : IServiceService
    {
        public ServiceService()
        {
            
        }
        public async Task<ApiResponseBase<List<ServiceDto>>> GetListPagingAsync(BaseFilterPagingDto filter)
        {
            return await RequestClient.PostAPIAsync<ApiResponseBase<List<ServiceDto>>>("services/filters", filter);
        }
        public async Task<ApiResponseBase<List<ServiceDto>>> GetListAsync()
        {
            return await RequestClient.GetAPIAsync<ApiResponseBase<List<ServiceDto>>>("services");
        }
        public async Task<ApiResponseBase<ServiceDto>> GetByIdAsync(Guid id)
        {
            return await RequestClient.GetAPIAsync<ApiResponseBase<ServiceDto>>($"services/{id}");
        }

        public async Task<ApiResponseBase<ServiceDto>> CreateAsync(CreateUpdateServiceDto input)
        {
            ApiResponseBase<ServiceDto> result = new ApiResponseBase<ServiceDto>();
            try
            {
                 result = await RequestClient.PostAPIAsync<ApiResponseBase<ServiceDto>>("services", input);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;

        }

        public async Task<ApiResponseBase<ServiceDto>> UpdateAsync(CreateUpdateServiceDto input, Guid id)
        {
            return await RequestClient.PutAPIAsync<ApiResponseBase<ServiceDto>>($"services/{id}" , input);
        }

        public async Task<ApiResponseBase<ServiceDto>> DeleteAsync(Guid id)
        {
            return  await RequestClient.DeleteAPIAsync<ApiResponseBase<ServiceDto>>($"services/{id}");
        }

        public Task<ApiResponseBase<int>> IncreaseViewsTotalAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}