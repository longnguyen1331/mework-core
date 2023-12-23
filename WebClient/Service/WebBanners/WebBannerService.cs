using Contract.WebBanners;
using WebClient.RequestHttp;

namespace WebClient.Service.WebBanners
{
    public class WebBannerService : IWebBannerService
    {
        public WebBannerService()
        {
            
        }
        public async Task<List<WebBannerDto>> GetListAsync()
        {
            return await RequestClient.GetAPIAsync<List<WebBannerDto>>("webBanners");
        }

        public async Task<WebBannerDto> CreateAsync(CreateUpdateWebBannerDto input)
        {
            return await RequestClient.PostAPIAsync<WebBannerDto>("webBanners", input);

        }

        public async Task<WebBannerDto> UpdateAsync(CreateUpdateWebBannerDto input, Guid id)
        {
            return await RequestClient.PutAPIAsync<WebBannerDto>($"webBanners/{id}" , input);

        }

        public async Task DeleteAsync(Guid id)
        { 
            await RequestClient.DeleteAPIAsync<Task>($"webBanners/{id}");
        }
    }
}