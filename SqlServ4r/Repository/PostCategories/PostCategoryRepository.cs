using Domain.PostCategories;
using SqlServ4r.EntityFramework;
using SqlServ4r.RepGenerationPatten;
using JetBrains.Annotations;
using Volo.Abp.DependencyInjection;

namespace SqlServ4r.Repository.PostCategories
{
	public class PostCategoryRepository : GenericRepository<PostCategory, Guid>, ITransientDependency
	{
		public PostCategoryRepository([NotNull] MeworkCoreContext context) : base(context)
		{
		}
	}
}
