using System;
using System.Collections.Generic;
using Contract.AppConfigs;
using Domain.AppConfigs;
using JetBrains.Annotations;
using SqlServ4r.EntityFramework;
using SqlServ4r.RepGenerationPatten;
using Volo.Abp.DependencyInjection;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SqlServ4r.Repository.AppConfigs
{
    public class AppConfigRepository  : GenericRepository<AppConfig, Guid>, ITransientDependency
    {
        public AppConfigRepository([NotNull] MeworkCoreContext context) : base(context)
        {
        }


        public async Task<AppConfig> GetAppliedConfigWithFile( )
        {
            var config = await _context.AppConfigs.FirstOrDefaultAsync(x => x.IsApply);

            if (config != null)
            {
                _context.Entry(config).Reference(o => o.IconFile).Load();
                _context.Entry(config).Reference(o => o.LogoFile).Load();
            }
            return config;
        }

    }
}