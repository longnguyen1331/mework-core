using Contract;
using Contract.WebMenus;
using Core.Const;
using Core.Exceptions;
using Domain.WebMenus;
using Microsoft.EntityFrameworkCore;
using SqlServ4r.Repository.WebMenus;
using System.Net;
using Volo.Abp.DependencyInjection;

namespace Application.WebMenus
{
    public class WebMenuService : ServiceBase, IWebMenuService, ITransientDependency
    {
        private readonly WebMenuRepository _webMenuRepository;


        public WebMenuService(WebMenuRepository webMenuRepository)
        {
            _webMenuRepository = webMenuRepository;
        }

        public async Task<WebMenuDto> CreateAsync(CreateUpdateWebMenuDto input)
        {
            var webMenu = ObjectMapper.Map<CreateUpdateWebMenuDto, WebMenu>(input);
            await _webMenuRepository.AddAsync(webMenu);


            return ObjectMapper.Map<WebMenu, WebMenuDto>(webMenu);
        }



        public async Task<WebMenuDto> UpdateAsync(CreateUpdateWebMenuDto input, Guid id)
        {

            var item = await _webMenuRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (item is null)
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }

            var webMenu = ObjectMapper.Map(input, item);
            await _webMenuRepository.UpdateAsync(webMenu);
            return ObjectMapper.Map<WebMenu, WebMenuDto>(webMenu);
        }

        public async Task DeleteAsync(Guid id)
        {
            var webMenu = await _webMenuRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (webMenu is null)
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }


            var webMenus =
                await _webMenuRepository.GetListAsync(x => x.ParentMenuId == id);

            foreach (var item in webMenus)
            {
                item.ParentMenuId = null;
            }

            _webMenuRepository.UpdateRange(webMenus);

            _webMenuRepository.Remove(webMenu);

        }

        public async Task<ApiResponseBase<List<WebMenuDto>>> GetListAsync()
        {
            ApiResponseBase<List<WebMenuDto>> result = new ApiResponseBase<List<WebMenuDto>>();

            try
            {
                var webMenus = _webMenuRepository.GetQueryable().
                   Include(x => x.Image).
                   OrderBy(x => x.ODX);
                result.Data = ObjectMapper.Map<List<WebMenu>, List<WebMenuDto>>(await webMenus.ToListAsync());
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        private (string Name, string? Code) TrimText(string name, string code)
        {
            return (name.Trim(), code?.Trim().ToUpper());
        }
    }
}