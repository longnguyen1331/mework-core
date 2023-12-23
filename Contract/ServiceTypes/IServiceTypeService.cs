using Contract.ServiceTypes;

namespace Contract.ServiceTypeServices
{
    public interface IServiceTypeService
    {
        Task<ServiceTypeDto> CreateAsync(CreateUpdateServiceTypeDto input);
        Task<ServiceTypeDto> UpdateAsync(CreateUpdateServiceTypeDto input,Guid id);
        Task DeleteAsync(Guid id);
        Task<List<ServiceTypeDto>> GetListAsync();
        

    }
}