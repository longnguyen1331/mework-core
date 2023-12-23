using System;
using System.IO;
using Domain.StaticFiles;
using JetBrains.Annotations;
using SqlServ4r.EntityFramework;
using SqlServ4r.RepGenerationPatten;
using Volo.Abp.DependencyInjection;

namespace SqlServ4r.Repository.Files
{
    public class StaticFileRepository : GenericRepository<StaticFile, Guid>, ITransientDependency
    {
        public StaticFileRepository([NotNull] MeworkCoreContext context) : base(context)
        {
        }
    }
}