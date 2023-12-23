using Contract.PostCategories;
using WebClient.RequestHttp;

namespace WebClient.Service.PostCategories
{
	public class PostCategoryService : IPostCategoryService
	{
		public async Task<PostCategoryDto> CreateAsync(CreateUpdatePostCategoryDto input)
		{
			return await RequestClient.PostAPIAsync<PostCategoryDto>($"postCategories", input);
		}

		public async Task DeleteAsync(Guid id)
		{
			await RequestClient.DeleteAPIAsync<Task>($"postCategories/{id}");
		}

		public async Task<List<PostCategoryDto>> GetListAsync()
		{
			return await RequestClient.GetAPIAsync<List<PostCategoryDto>>($"postCategories");
		}

		public async Task<PostCategoryDto> UpdateAsync(CreateUpdatePostCategoryDto input, Guid id)
		{
			return await RequestClient.PutAPIAsync<PostCategoryDto>($"postCategories/{id}", input);
		}
	}
}
