using Domain.Backups;
using JetBrains.Annotations;
using SqlServ4r.EntityFramework;
using SqlServ4r.RepGenerationPatten;
using Volo.Abp.DependencyInjection;

namespace SqlServ4r.Repository.Backups
{
    public class BackupRepository : GenericRepository<Backup, Guid>, ITransientDependency
    {
        public BackupRepository([NotNull] MeworkCoreContext context) : base(context)
        {
        }
    }
}