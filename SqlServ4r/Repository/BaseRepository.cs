using SqlServ4r.EntityFramework;
using Volo.Abp.DependencyInjection;

namespace SqlServ4r.Repository
{
    public class BaseRepository : ITransientDependency
    {
        public readonly MeworkCoreContext _context; 

        public BaseRepository(MeworkCoreContext context)
        {
            _context = context;
        }
    }
}