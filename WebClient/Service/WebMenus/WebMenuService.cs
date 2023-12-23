using Contract;
using Contract.WebMenus;
using WebClient.RequestHttp;

namespace WebClient.Service.WebMenus
{
    public class WebMenuService : IWebMenuService
    {
        
        public WebMenuService()
        {
            
        }
        
        public async Task<ApiResponseBase<List<WebMenuDto>>> GetListAsync()
        {
            return await RequestClient.GetAPIAsync<ApiResponseBase<List<WebMenuDto>>>("webMenus");
        }

        public async Task<WebMenuDto> CreateAsync(CreateUpdateWebMenuDto input)
        {
            return await RequestClient.PostAPIAsync<WebMenuDto>("webMenus",input);

        }

        public async Task<WebMenuDto> UpdateAsync(CreateUpdateWebMenuDto input, Guid id)
        {
            return await RequestClient.PutAPIAsync<WebMenuDto>($"webMenus/{id}" , input);

        }

        public async Task DeleteAsync(Guid id)
        { 
            await RequestClient.DeleteAPIAsync<Task>($"webMenus/{id}");
        }
    }
}