using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contract.Departments
{
    public interface IDepartmentService
    {
        Task<DepartmentDto> CreateAsync(CreateUpdateDepartmentDto input);
        Task<DepartmentDto> UpdateAsync(CreateUpdateDepartmentDto input,Guid id);
        Task DeleteAsync(Guid id);
        Task<List<DepartmentDto>> GetListAsync();
        

    }
}