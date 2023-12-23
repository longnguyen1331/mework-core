using Core.Enum;

namespace Contract.WebBanners
{
    public class WebBannerDto
    {
        public Guid Id { get; set; }
        public int ODX { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string BannerUrl { get; set; }
        public string MobileUrl { get; set; }
        public WebBannerType BannerType { get; set; }
        public string? ButtonText { get; set; }
        public string? UrlRef { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsVisibled { get; set; }
        public string WebsiteBannerUrl { get; set; }
        public string MobileBannerUrl { get; set; }
        public Guid? WebsiteBannerId { get; set; }
        public Guid? MobileBannerId { get; set; }
        public string? BannerHref { get; set; }
    }
}