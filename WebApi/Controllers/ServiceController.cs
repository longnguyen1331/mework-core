using Application.Services;
using Contract;
using Contract.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/services/")]
    [Authorize]
    public class ServiceController(ServiceService _serviceService)
    {
        [HttpPost]
        public async Task<ApiResponseBase<ServiceDto>> CreateAsync(CreateUpdateServiceDto input)
        {
            return await _serviceService.CreateAsync(input); 
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ApiResponseBase<ServiceDto>> UpdateAsync(CreateUpdateServiceDto input, Guid id)
        {
            return  await _serviceService.UpdateAsync(input,id);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ApiResponseBase<ServiceDto>> DeleteAsync(Guid id)
        {
            return await _serviceService.DeleteAsync(id);
        }

        [HttpGet]
        public async Task<ApiResponseBase<List<ServiceDto>>> GetListAsync()
        {
            return await _serviceService.GetListAsync();
        }

        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<ApiResponseBase<ServiceDto>> GetByIdAsync(Guid id)
        {
            return await _serviceService.GetByIdAsync(id);
        }

        [HttpPost]
        [Route("filters")]
        public async Task<ApiResponseBase<List<ServiceDto>>> GetListPagingAsync(BaseFilterPagingDto filter)
        {
            return await _serviceService.GetListPagingAsync(filter);
        }

        [HttpPost]
        [Route("advance-filters")]
        [AllowAnonymous]
        public async Task<ApiResponseBase<ServiceSearchResultDto>> GetListPagingAsync([FromBody]ServiceFilterPagingDto filter)
        {
            return await _serviceService.GetListPagingAsync(filter);
        }

        [HttpGet]
        [Route("related-service/{id}")]
        [AllowAnonymous]
        public async Task<ApiResponseBase<List<ServiceDto>>> GetRelatedListAsync(Guid id)
        {
            return await _serviceService.GetRelatedListAsync(id);
        }

        [HttpPut]
        [Route("increase-views/{id}")]
        [AllowAnonymous]
        public async Task<ApiResponseBase<int>> IncreaseViewsTotalAsync(Guid id)
        {
            return await _serviceService.IncreaseViewsTotalAsync(id);
        }
    }
}