using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contract.ServiceTypes;
using Contract.ServiceTypeServices;
using WebClient.RequestHttp;

namespace WebClient.Service.ServiceTypes
{
    public class ServiceTypeService : IServiceTypeService
    {
        public ServiceTypeService()
        {
            
        }
        
        public async Task<List<ServiceTypeDto>> GetListAsync()
        {
            return await RequestClient.GetAPIAsync<List<ServiceTypeDto>>("serviceTypes");
        }

        public async Task<ServiceTypeDto> CreateAsync(CreateUpdateServiceTypeDto input)
        {
            return await RequestClient.PostAPIAsync<ServiceTypeDto>("serviceTypes", input);

        }

        public async Task<ServiceTypeDto> UpdateAsync(CreateUpdateServiceTypeDto input, Guid id)
        {
            return await RequestClient.PutAPIAsync<ServiceTypeDto>($"serviceTypes/{id}" , input);
        }

        public async Task DeleteAsync(Guid id)
        { 
            await RequestClient.DeleteAPIAsync<Task>($"serviceTypes/{id}");
        }
    }
}