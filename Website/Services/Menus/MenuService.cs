using Contract.WebMenus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Website.Models;
using Website.Models.ResponseModels;
using Website.Utils;

namespace Website.Services.Menus
{
    public class MenuService : IMenuService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RemoteAPIConfig _remoteOptions;

        private string CacheName = "MenusCache";

        public MenuService(IHttpClientFactory httpClientFactory,
                           IOptions<RemoteAPIConfig> remoteOptions)
        {
            if (remoteOptions is null)
            {
                throw new ArgumentNullException(nameof(remoteOptions));
            }

            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _remoteOptions = remoteOptions.Value;
        }

        public async Task<MenuResponseModel> GetListMenuAsync(string? currentURL)
        {
            MenuResponseModel result = new MenuResponseModel();

            var client = _httpClientFactory.CreateClient("Website");
            var response = await client.GetStringAsync(_remoteOptions.GetWebsiteMenu);

           var menus = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<List<WebMenuDto>>(response) : null;
            List<WebMenuDto> webMenus = new List<WebMenuDto>();
            if (menus != null && menus.Any())
            {
                result.CurrentMenu = menus.FirstOrDefault(s => !string.IsNullOrEmpty(currentURL) && !currentURL.Equals("/") && StringUtil.GenerateMenuHref(s).Equals(currentURL));

                foreach (var menu in menus.Where(x => x.ParentMenuId == null))
                {
                    webMenus.Add(GetMenus(menu, menus));
                }
            }

            result.Items = webMenus;

            return result;
        }

        private WebMenuDto GetMenus(WebMenuDto menu, List<WebMenuDto> menus)
        {
            var childs = menus.Where(x => x.ParentMenuId == menu.Id).ToList();
            if (childs.Any())
            {
                foreach (var child in childs)
                {
                    menu.ChildWebMenu.Add(GetMenus(child, menus));
                }
            }

            return menu;
        }
    }
}
