using Website.Models.ResponseModels;

namespace Website.Services.Menus
{
    public interface IMenuService
    {
        Task<MenuResponseModel> GetListMenuAsync(string? currentURL);
    }
}
