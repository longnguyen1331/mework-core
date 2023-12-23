using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contract.Departments;
using WebClient.RequestHttp;

namespace WebClient.Service.Departments
{
    public class DepartmentService : IDepartmentService
    {
        
        public DepartmentService()
        {
            
        }
        
        public async Task<List<DepartmentDto>> GetListAsync()
        {
            return await RequestClient.GetAPIAsync<List<DepartmentDto>>("department");
        }

        public async Task<DepartmentDto> CreateAsync(CreateUpdateDepartmentDto input)
        {
            return await RequestClient.PostAPIAsync<DepartmentDto>("department",input);

        }

        public async Task<DepartmentDto> UpdateAsync(CreateUpdateDepartmentDto input, Guid id)
        {
            return await RequestClient.PutAPIAsync<DepartmentDto>($"department/{id}" , input);

        }

        public async Task DeleteAsync(Guid id)
        { 
            await RequestClient.DeleteAPIAsync<Task>($"department/{id}");
        }
    }
}