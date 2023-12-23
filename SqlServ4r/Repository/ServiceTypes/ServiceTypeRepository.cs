using System;
using Domain.ServiceTypes;
using JetBrains.Annotations;
using SqlServ4r.EntityFramework;
using SqlServ4r.RepGenerationPatten;
using Volo.Abp.DependencyInjection;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SqlServ4r.Repository.ServiceTypes
{
    public class ServiceTypeRepository  : GenericRepository<ServiceType, Guid>, ITransientDependency
    {
        public ServiceTypeRepository([NotNull] MeworkCoreContext context) : base(context)
        {
        }

        public async Task<ServiceType> GetAppliedServiceTypeWithFile()
        {
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync();

            if (serviceType != null)
            {
                _context.Entry(serviceType).Reference(o => o.ImageFile).Load();
            }
            return serviceType;
        }

        public (bool ExistCode, bool ExistName) CheckDuplicateInformation(string code, string name)
        {
            var query = from user in _context.ServiceTypes
                select new ValueTuple<bool,bool>(
                    _context.ServiceTypes.Any(x => x.Code == code && x.Code != null),
                    _context.ServiceTypes.Any(x => x.Name== name)
                );
            
            return query.FirstOrDefault();
        }
        
        public (bool ExistCode, bool ExistName) CheckDuplicateInformation(string code, string name,Guid id)
        {
            var query = from user in _context.ServiceTypes
                select new ValueTuple<bool,bool>(
                    _context.ServiceTypes.Any(x => x.Code == code && x.Code != null && x.Id != id),
                    _context.ServiceTypes.Any(x => x.Name== name && x.Id != id)
                );
            
            return query.FirstOrDefault();
        }
    }
}