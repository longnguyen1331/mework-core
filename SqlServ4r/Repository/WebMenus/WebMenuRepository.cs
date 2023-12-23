using System;
using Domain.WebMenus;
using JetBrains.Annotations;
using SqlServ4r.EntityFramework;
using SqlServ4r.RepGenerationPatten;
using Volo.Abp.DependencyInjection;
using System.Linq;

namespace SqlServ4r.Repository.WebMenus
{
    public class WebMenuRepository  : GenericRepository<WebMenu, Guid>, ITransientDependency
    {
        public WebMenuRepository([NotNull] MeworkCoreContext context) : base(context)
        {
        }
        
        public (bool ExistCode, bool ExistName) CheckDuplicateInformation(string menuType, string name)
        {
            var query = from user in _context.WebMenus
                select new ValueTuple<bool,bool>(
                    _context.WebMenus.Any(x => x.MenuType == menuType && x.MenuType != null),
                    _context.WebMenus.Any(x => x.Name== name)
                );
            
            return query.FirstOrDefault();
        }
        
        public (bool ExistCode, bool ExistName) CheckDuplicateInformation(string menuType, string name,Guid id)
        {
            var query = from user in _context.WebMenus
                        select new ValueTuple<bool,bool>(
                    _context.WebMenus.Any(x => x.MenuType == menuType && x.MenuType != null && x.Id != id),
                    _context.WebMenus.Any(x => x.Name== name && x.Id != id)
                );
            
            return query.FirstOrDefault();
        }
    }
}