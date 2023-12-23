using Domain.Services;
using JetBrains.Annotations;
using SqlServ4r.EntityFramework;
using SqlServ4r.RepGenerationPatten;
using Volo.Abp.DependencyInjection;

namespace SqlServ4r.Repository.Services
{
    public class ServiceRepository  : GenericRepository<Service, Guid>, ITransientDependency
    {
        public ServiceRepository([NotNull] MeworkCoreContext context) : base(context)
        {
        }

        public (bool ExistCode, bool ExistName) CheckDuplicateInformation(string code, string name)
        {
            var query = from user in _context.Services
                select new ValueTuple<bool,bool>(
                    _context.Services.Any(x => x.Code == code && x.Code != null),
                    _context.Services.Any(x => x.Name== name)
                );
            
            return query.FirstOrDefault();
        }
        
        public (bool ExistCode, bool ExistName) CheckDuplicateInformation(string code, string name,Guid id)
        {
            var query = from user in _context.Services
                select new ValueTuple<bool,bool>(
                    _context.Services.Any(x => x.Code == code && x.Code != null && x.Id != id),
                    _context.Services.Any(x => x.Name== name && x.Id != id)
                );
            
            return query.FirstOrDefault();
        }
    }
}