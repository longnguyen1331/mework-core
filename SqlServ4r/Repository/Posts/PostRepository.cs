using Domain.Posts;
using SqlServ4r.EntityFramework;
using SqlServ4r.RepGenerationPatten;
using JetBrains.Annotations;
using Volo.Abp.DependencyInjection;

namespace SqlServ4r.Repository.Posts
{
    public class PostRepository : GenericRepository<Post, Guid>, ITransientDependency
    {
        public PostRepository([NotNull] MeworkCoreContext context) : base(context)
        {
        }
    }
}
