using Contract.Common.Excels;

namespace Contract.Posts
{
    public interface IPostService
    {
        Task<ApiResponseBase<PostDto>> CreateAsync(CreateUpdatePostDto input);
        Task<ApiResponseBase<PostDto>> UpdateAsync(CreateUpdatePostDto input, Guid id);
        Task<ApiResponseBase<PostDto>> DeleteAsync(Guid id);
        Task<ApiResponseBase<List<PostDto>>> GetListAsync();
        Task<ApiResponseBase<PostDto>> GetByIdAsync(Guid id);
        Task<ApiResponseBase<List<PostDto>>> GetListPagingAsync(BaseFilterPagingDto filter);
        Task<ApiResponseBase<byte[]>> ExportExcel(PostFitlerPagingDto filter);
        Task<ApiResponseBase<DataValidatorExcel>> CreatePostFromExcelFileAsync(Microsoft.AspNetCore.Http.IFormFile file);
        Task<ApiResponseBase<int>> IncreaseViewsTotalAsync(Guid id);
        Task<ApiResponseBase<List<string>>> GetListTags(int? skip, int? take);
    }
}
