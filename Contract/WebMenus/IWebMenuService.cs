using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contract.WebMenus
{
    public interface IWebMenuService
    {
        Task<WebMenuDto> CreateAsync(CreateUpdateWebMenuDto input);
        Task<WebMenuDto> UpdateAsync(CreateUpdateWebMenuDto input,Guid id);
        Task DeleteAsync(Guid id);
        Task<ApiResponseBase<List<WebMenuDto>>> GetListAsync();
        

    }
}