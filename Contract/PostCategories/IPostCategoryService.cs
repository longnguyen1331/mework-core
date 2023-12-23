namespace Contract.PostCategories
{
	public interface IPostCategoryService
	{
		Task<PostCategoryDto> CreateAsync(CreateUpdatePostCategoryDto input);
		Task<PostCategoryDto> UpdateAsync(CreateUpdatePostCategoryDto input, Guid id);
		Task DeleteAsync(Guid id);
		Task<List<PostCategoryDto>> GetListAsync();
	}
}
