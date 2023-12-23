namespace Contract.WebBanners
{
    public interface IWebBannerService
    {
        Task<WebBannerDto> CreateAsync(CreateUpdateWebBannerDto input);
        Task<WebBannerDto> UpdateAsync(CreateUpdateWebBannerDto input,Guid id);
        Task DeleteAsync(Guid id);
        Task<List<WebBannerDto>> GetListAsync();
        

    }
}