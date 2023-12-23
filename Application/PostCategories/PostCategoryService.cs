using Contract;
using Contract.PostCategories;
using Contract.Posts;
using Core.Const;
using Core.Exceptions;
using Domain.PostCategories;
using Microsoft.EntityFrameworkCore;
using SqlServ4r.Repository.PostCategories;
using System.Net;
using Volo.Abp.DependencyInjection;

namespace Application.PostCategories
{
	public class PostCategoryService : ServiceBase, IPostCategoryService, ITransientDependency
	{
		private readonly PostCategoryRepository _postCategoryRepository;

		public PostCategoryService(PostCategoryRepository postCategoryRepository)
		{
			_postCategoryRepository = postCategoryRepository;
		}

		public async Task<PostCategoryDto> CreateAsync(CreateUpdatePostCategoryDto input)
		{
			(input.Name, input.SeoTitle, input.Slug, input.SeoKeyword, input.SeoDescription) = TrimText(input.Name, input.SeoTitle, input.Slug, input.SeoKeyword, input.SeoDescription);

			var postCategory = ObjectMapper.Map<PostCategory>(input);

			await _postCategoryRepository.AddAsync(postCategory);

			return ObjectMapper.Map<PostCategory, PostCategoryDto>(postCategory);
		}

		public async Task DeleteAsync(Guid id)
		{
			var postCategory = await _postCategoryRepository.FirstOrDefaultAsync(x => x.Id == id);

			if (postCategory == null)
				throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);

			postCategory.IsDeleted = true;
            await _postCategoryRepository.UpdateAsync(postCategory);
		}

		public async Task<List<PostCategoryDto>> GetListAsync()
		{
			var postCategories = await _postCategoryRepository.GetQueryable().Where(x => x.IsDeleted == false).Include(x => x.Image).ToListAsync();

			return ObjectMapper.Map<List<PostCategory>, List<PostCategoryDto>>(postCategories);
		}
		
		public async Task<List<PostCategoryDto>> GetListAsync(BaseFilterPagingDto filter)
		{
			var postCategories = _postCategoryRepository
				.GetQueryable()
                .Include(x => x.Image)
                .Where(x => x.IsDeleted == false && (!string.IsNullOrEmpty(filter.FilterText) ? x.Name.Contains(filter.FilterText) : 1==1));

			if (filter.Take > 0)
                postCategories = postCategories.Skip(filter.Skip).Take(filter.Take);

			var result = await postCategories.OrderBy(x => x.ODX).OrderByDescending(x => x.CreatedDate).ToListAsync();

            return ObjectMapper.Map<List<PostCategory>, List<PostCategoryDto>>(result);
		}
		
		public async Task<List<PostCategoryDto>> GetListAsync(PostCategoryFitlerPagingDto filter)
		{
			List<Guid> ids = new List<Guid>();

			if (!string.IsNullOrEmpty(filter.Ids))
                ids = filter.Ids.Split(',').Select(x => Guid.TryParse(x, out Guid guid) ? guid : Guid.Empty).ToList();

            var postCategories = _postCategoryRepository
				.GetQueryable()
                .Include(x => x.Image)
                .Where(x =>
					x.IsDeleted == false
					&& (!string.IsNullOrEmpty(filter.FilterText) ? x.Name.Contains(filter.FilterText) : 1==1)
					&& (ids.Any() ? ids.Any(s => s == x.Id) : true)
                );

			if (filter.Take > 0)
                postCategories = postCategories.Skip(filter.Skip).Take(filter.Take);

			var result = await postCategories.OrderBy(x => x.ODX).OrderByDescending(x => x.CreatedDate).ToListAsync();

            return ObjectMapper.Map<List<PostCategory>, List<PostCategoryDto>>(result);
		}

		public async Task<PostCategoryDto> UpdateAsync(CreateUpdatePostCategoryDto input, Guid id)
		{
			var postCategory = await _postCategoryRepository.FirstOrDefaultAsync(x => x.Id == id);

			if (postCategory == null)
				throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);

			var entity = ObjectMapper.Map(input, postCategory);
			await _postCategoryRepository.UpdateAsync(entity);

			return ObjectMapper.Map<PostCategoryDto>(entity);
		}

		private (string Name, string SeoTitle, string Slug, string SeoKeyword, string SeoDescription) TrimText(string name, string seoTitle, string slug, string seoKeyword, string seoDescription)
		{
			return (name.Trim(), seoTitle.Trim(), slug.Trim(), seoKeyword.Trim(), seoDescription.Trim());
		}
	}
}
