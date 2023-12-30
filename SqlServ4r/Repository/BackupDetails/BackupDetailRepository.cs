using Domain.BackupDetails;
using JetBrains.Annotations;
using SqlServ4r.EntityFramework;
using SqlServ4r.RepGenerationPatten;
using Volo.Abp.DependencyInjection;

namespace SqlServ4r.Repository.BackupDetails
{
    public class BackupDetailRepository : GenericRepository<BackupDetail, Guid>, ITransientDependency
    {
        public BackupDetailRepository([NotNull] MeworkCoreContext context) : base(context)
        {
        }
    }
}