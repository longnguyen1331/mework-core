using Application.WebBanners;
using Contract.WebBanners;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{

    [ApiController]
    [Route("api/webBanners/")]
    [Authorize]
    public class WebBannerController : IWebBannerService
    {
        private WebBannerService _webBannerService;
        public WebBannerController(WebBannerService webBannerService)
        {
            _webBannerService = webBannerService;
        }
        
        [HttpPost]
        public async Task<WebBannerDto> CreateAsync(CreateUpdateWebBannerDto input)
        {
            return await _webBannerService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<WebBannerDto> UpdateAsync(CreateUpdateWebBannerDto input, Guid id)
        {
            return  await _webBannerService.UpdateAsync(input,id);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _webBannerService.DeleteAsync(id);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<List<WebBannerDto>> GetListAsync()
        {
            return await _webBannerService.GetListAsync();
        }
    }
}