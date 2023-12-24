using Contract.WebBanners;
using Contract.WebMenus;
using Core.Enum;
using Core.Extension;

namespace Website.Utils
{
    public static class StringUtil
    {
        public static string GenerateMenuHref(WebMenuDto? menu)
        {
            string url = "/";

            if (menu != null && int.TryParse(menu.MenuType, out int menuType))
            {
                string menySlug = $"{menu.Slug}{menu.MenuSuffix}{menu.UrlRef}";

                switch (menuType)
                {
                    case (int)WebMenuType.PostDetail:
                        url += $"post/{menySlug}";
                        break;

                    case (int)WebMenuType.PostCategory:
                        url += $"postCategory/{menySlug}";
                        break;

                    case (int)WebMenuType.ServiceType:
                        url += $"service/{menySlug}";
                        break;

                    case (int)WebMenuType.ServiceDetail:
                        url += $"serviceDetail/{menySlug}";
                        break;

                    //case (int)WebMenuType.Doctor:
                    //    url += $"doctor/{menySlug}";
                    //    break;
                    default:
                        if (!string.IsNullOrEmpty(menu.UrlRef) && (menu.UrlRef.StartsWith("/") || menu.UrlRef.StartsWith("http:") || menu.UrlRef.StartsWith("https:")))
                            url = menu.UrlRef;
                        else
                            url += menu.UrlRef;
                        break;
                }
            }

            return url;
        }
        public static string GenerateBannerHref(WebBannerDto? banner)
        {
            string url = "/";

            if (banner != null)
            {
                int bannerType = (int)banner.BannerType;
                string enumDescription;
                string slug = banner.Title.GenerateSlug();

                switch (bannerType)
                {
                    case (int)WebBannerType.News:
                        enumDescription = WebMenuType.PostDetail.GetDescriptionOrName();
                        url += $"post/{slug}{enumDescription}{banner.Id}";
                        break;

                    case (int)WebBannerType.Service:
                        enumDescription = WebMenuType.ServiceDetail.GetDescriptionOrName();
                        url += $"service/{slug}{enumDescription}{banner.Id}";
                        break;

                    default:
                        if (!string.IsNullOrEmpty(banner.UrlRef) && (banner.UrlRef.StartsWith("/") || banner.UrlRef.StartsWith("http:") || banner.UrlRef.StartsWith("https:")))
                            url = banner.UrlRef;
                        else
                            url += banner.UrlRef;
                        break;
                }
            }

            return url;
        }
    }
}
