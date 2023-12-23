using Core.Enum;
using Domain.AppConfigs;
using Domain.StaticFiles;
using System.ComponentModel.DataAnnotations;

namespace Domain.WebBanners
{
    public class WebBanner : BaseEntity
    {
        public WebBanner() 
        { 
            ODX = 0;
            IsDeleted = false;
            IsVisibled = false;
        }

        [MaxLength(128)]
        public string Title { get; set; }
        public string? Description { get; set; }
        public WebBannerType BannerType { get; set; }
        [MaxLength(128)]
        public string? ButtonText { get; set; }
        [MaxLength(128)]
        public string? UrlRef { get; set; }

        public int ODX { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsVisibled { get; set; }
        public Guid? WebsiteBannerId { get; set; }
        public Guid? MobileBannerId { get; set; }
        public StaticFile WebsiteBanner { get; set; }
        public StaticFile MobileBanner { get; set; }
    }
}