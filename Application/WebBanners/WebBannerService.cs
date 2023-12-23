using Contract.WebBanners;
using Core.Const;
using Core.Exceptions;
using Domain.WebBanners;
using Microsoft.EntityFrameworkCore;
using SqlServ4r.Repository.Files;
using SqlServ4r.Repository.Users;
using SqlServ4r.Repository.WebBanners;
using System.Net;
using Volo.Abp.DependencyInjection;

namespace Application.WebBanners
{
    public class WebBannerService : ServiceBase,IWebBannerService,ITransientDependency
    {
       private readonly WebBannerRepository _webBannerRepository;
       private readonly StaticFileRepository _staticFileRepository;
       private readonly UserRepository _userRepository;


        public WebBannerService(
            WebBannerRepository webBannerRepository,
            StaticFileRepository staticFileRepository,
            UserRepository _userRepository)
        {
            _webBannerRepository = webBannerRepository;
            _staticFileRepository = staticFileRepository;
            _userRepository = _userRepository;
        }

        public async Task<WebBannerDto> CreateAsync(CreateUpdateWebBannerDto input)
        {

            var webBanner = ObjectMapper.Map<CreateUpdateWebBannerDto, WebBanner>(input);
            webBanner.CreatedDate = DateTime.Now;   
            webBanner.ModifiedDate = DateTime.Now;  
            await _webBannerRepository.AddAsync(webBanner);
            return ObjectMapper.Map<WebBanner,WebBannerDto>(webBanner);
        }

        public async Task<WebBannerDto> UpdateAsync(CreateUpdateWebBannerDto input, Guid id)
        {
            var item = await _webBannerRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (item is null)
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }
            
            var webBanner = ObjectMapper.Map(input,item);
            webBanner.ModifiedDate = DateTime.Now;

            await _webBannerRepository.UpdateAsync(webBanner);
            return ObjectMapper.Map<WebBanner,WebBannerDto>(webBanner);
        }

 

        public async Task DeleteAsync(Guid id)
        {
            var webBanner = await _webBannerRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (webBanner is null)
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }

            webBanner.IsDeleted = true;
            await _webBannerRepository.UpdateAsync(webBanner);
        }

        public async Task<List<WebBannerDto>> GetListAsync()
        {
            var webBanners = await (from webBanner in _webBannerRepository.GetQueryable()
                        join desktopImage in _staticFileRepository.GetQueryable()
                          on webBanner.WebsiteBannerId equals desktopImage.Id into ps_desktop
                        from imgDesktop in ps_desktop.DefaultIfEmpty()
                        join mobileImage in _staticFileRepository.GetQueryable()
                          on webBanner.MobileBannerId equals mobileImage.Id into ps_mobile
                        from imgMobile in ps_mobile.DefaultIfEmpty()

                        where webBanner.IsDeleted == false 
                        select new WebBannerDto
                        {
                            Id = webBanner.Id,
                            WebsiteBannerId = imgDesktop.Id,
                            WebsiteBannerUrl = imgDesktop.URL,
                            MobileBannerId = imgMobile.Id,
                            MobileBannerUrl = imgMobile.URL,
                            ODX = webBanner.ODX,
                            Title = webBanner.Title,
                            BannerType = webBanner.BannerType,
                            Description = webBanner.Description,
                            IsDeleted = webBanner.IsDeleted,
                            IsVisibled = webBanner.IsVisibled,
                            ButtonText = webBanner.ButtonText,
                            UrlRef = webBanner.UrlRef,
                        }).OrderBy(x => x.ODX).AsNoTracking().ToListAsync();

            return webBanners;
        }
    }
}