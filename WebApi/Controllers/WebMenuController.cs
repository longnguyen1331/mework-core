using Application.WebMenus;
using Contract;
using Contract.WebMenus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{

    [ApiController]
    [Route("api/webMenus/")]
    [Authorize]
    public class WebMenuController : IWebMenuService
    {
        private WebMenuService _webMenuService;
        public WebMenuController(WebMenuService positionService)
        {
            _webMenuService = positionService;
        }
        
        [HttpPost]
        public async Task<WebMenuDto> CreateAsync(CreateUpdateWebMenuDto input)
        {
            return await _webMenuService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<WebMenuDto> UpdateAsync(CreateUpdateWebMenuDto input, Guid id)
        {
            return  await _webMenuService.UpdateAsync(input,id);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _webMenuService.DeleteAsync(id);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResponseBase<List<WebMenuDto>>> GetListAsync()
        {
            return await _webMenuService.GetListAsync();
        }
    }
}