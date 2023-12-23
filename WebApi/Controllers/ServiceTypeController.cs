using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.ServiceTypes;
using Contract.ServiceTypes;
using Contract.ServiceTypeServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/serviceTypes/")]
    [Authorize]
    public class ServiceTypeController : IServiceTypeService
    {
        private ServiceTypeService _serviceTypeService;
        public ServiceTypeController(ServiceTypeService serviceTypeService)
        {
            _serviceTypeService = serviceTypeService;
        }
        
        [HttpPost]
        public async Task<ServiceTypeDto> CreateAsync(CreateUpdateServiceTypeDto input)
        {
            return await _serviceTypeService.CreateAsync(input); 
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ServiceTypeDto> UpdateAsync(CreateUpdateServiceTypeDto input, Guid id)
        {
            return  await _serviceTypeService.UpdateAsync(input,id);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _serviceTypeService.DeleteAsync(id);
        }

        [HttpGet]
        public async Task<List<ServiceTypeDto>> GetListAsync()
        {
            return await _serviceTypeService.GetListAsync();
        }

        [HttpPost]
        [Route("filter")]
        [AllowAnonymous]
        public async Task<List<ServiceTypeDto>> GetListAsync(ServiceTypeFilterPagingDto filter)
        {
            return await _serviceTypeService.GetListAsync(filter);
        }
    }
}