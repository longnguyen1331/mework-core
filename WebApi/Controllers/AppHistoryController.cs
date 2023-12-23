using Application.AppHistorys;
using Contract;
using Contract.AppHistories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/appHistories/")]
    [Authorize]
    public class AppHistoryController : IAppHistoryService
    {
        private AppHistoryService _appHistoryService;
        public AppHistoryController(AppHistoryService appHistoryService)
        {
            _appHistoryService = appHistoryService;
        }
        
        [HttpPost]
        public async Task CreateAsync(CreateUpdateAppHistoryDto input)
        {
            await _appHistoryService.CreateAsync(input); 
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<AppHistoryDto> UpdateAsync(CreateUpdateAppHistoryDto input, Guid id)
        {
            return  await _appHistoryService.UpdateAsync(input,id);
        }

        [HttpGet]
        public async Task<ApiResponseBase<List<AppHistoryDto>>> GetListAsync()
        {
            return await _appHistoryService.GetListAsync();
        }

        [HttpPost] 
        [Route("search")]
        public async Task<ApiResponseBase<AppHistorySearchResponseDto>> GetListAsync(AppHistoryFilterPagingDto filter)
        {
            return await _appHistoryService.GetListAsync(filter);
        }
    }
}