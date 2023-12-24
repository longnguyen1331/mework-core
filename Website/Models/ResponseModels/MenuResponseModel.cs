using Contract.WebMenus;

namespace Website.Models.ResponseModels
{
    public class MenuResponseModel
    {
        public List<WebMenuDto>? Items { get; set; }
        public WebMenuDto? CurrentMenu { get; set; }
    }
}
