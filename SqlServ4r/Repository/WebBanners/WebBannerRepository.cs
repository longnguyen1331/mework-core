using Domain.WebBanners;
using JetBrains.Annotations;
using SqlServ4r.EntityFramework;
using SqlServ4r.RepGenerationPatten;
using Volo.Abp.DependencyInjection;

namespace SqlServ4r.Repository.WebBanners
{
    public class WebBannerRepository  : GenericRepository<WebBanner, Guid>, ITransientDependency
    {
        public WebBannerRepository([NotNull] MeworkCoreContext context) : base(context)
        {
        }
    }
}