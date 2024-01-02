using Application.Departments;
using Contract.Departments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{

    [ApiController]
    [Route("api/department/")]
    [Authorize]
    public class DepartmentController (DepartmentService _departmentService) //syntax .Net 8 => private readonly
    {
        [HttpPost]
        public async Task<DepartmentDto> CreateAsync(CreateUpdateDepartmentDto input)
        {
            return await _departmentService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<DepartmentDto> UpdateAsync(CreateUpdateDepartmentDto input, Guid id)
        {
            return  await _departmentService.UpdateAsync(input,id);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _departmentService.DeleteAsync(id);
        }

        [HttpGet]
        public async Task<List<DepartmentDto>> GetListAsync()
        {
            return await _departmentService.GetListAsync();
        }
    }
}