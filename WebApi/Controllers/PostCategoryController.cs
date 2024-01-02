using Application.PostCategories;
using Contract;
using Contract.PostCategories;
using Contract.Posts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
	[ApiController]
	[Route("api/postCategories/")]
	[Authorize]
	public class PostCategoryController(PostCategoryService _postCategory)
    {
        [HttpPost]
		public async Task<PostCategoryDto> CreateAsync(CreateUpdatePostCategoryDto input)
		{
			return await _postCategory.CreateAsync(input);
		}

		[HttpDelete]
		[Route("{id}")]
		public async Task DeleteAsync(Guid id)
		{
			await _postCategory.DeleteAsync(id);
		}

		[HttpGet]
		public async Task<List<PostCategoryDto>> GetListAsync()
		{
			return await _postCategory.GetListAsync();
		}
		
		[HttpPost]
		[Route("filter")]
		[AllowAnonymous]
		public async Task<List<PostCategoryDto>> GetListAsync(BaseFilterPagingDto filter)
		{
			return await _postCategory.GetListAsync(filter);
		}
		
		[HttpPost]
		[Route("advance-filters")]
		[AllowAnonymous]
		public async Task<List<PostCategoryDto>> GetListAsync(PostCategoryFitlerPagingDto filter)
		{
			return await _postCategory.GetListAsync(filter);
		}

		[HttpPut]
		[Route("{id}")]
		public async Task<PostCategoryDto> UpdateAsync(CreateUpdatePostCategoryDto input, Guid id)
		{
			return await _postCategory.UpdateAsync(input, id);
		}
	}
}
