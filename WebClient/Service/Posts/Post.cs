using Contract;
using Contract.Common.Excels;
using Contract.Posts;
using Microsoft.AspNetCore.Components.Forms;
using WebClient.RequestHttp;

namespace WebClient.Service.Posts
{
    public class PostService : IPostService
    {
        public async Task<ApiResponseBase<PostDto>> CreateAsync(CreateUpdatePostDto input)
        {
            return await RequestClient.PostAPIAsync<ApiResponseBase<PostDto>>("posts", input);
        }

        public async Task<ApiResponseBase<PostDto>> DeleteAsync(Guid id)
        {
           return  await RequestClient.DeleteAPIAsync<ApiResponseBase<PostDto>>($"posts/{id}");
        }

        public async Task<ApiResponseBase<List<PostDto>>> GetListAsync()
        {
            return await RequestClient.GetAPIAsync<ApiResponseBase<List<PostDto>>>("posts");
        }

        public async Task<ApiResponseBase<PostDto>> UpdateAsync(CreateUpdatePostDto input, Guid id)
        {
            return await RequestClient.PutAPIAsync<ApiResponseBase<PostDto>>($"posts/{id}", input);
        }
        public async Task<ApiResponseBase<PostDto>> GetByIdAsync(Guid id)
        {
            return await RequestClient.GetAPIAsync<ApiResponseBase<PostDto>>($"posts/{id}");
        }

        public async Task<ApiResponseBase<List<PostDto>>> GetListPagingAsync(BaseFilterPagingDto filter)
        {
            return await RequestClient.PostAPIAsync<ApiResponseBase<List<PostDto>>>("posts/filters", filter);
        }

        public async Task<ApiResponseBase<byte[]>> ExportExcel(PostFitlerPagingDto filter)
        {
            return await RequestClient.PostAPIAsync<ApiResponseBase<byte[]>>("posts/export-excel", filter);
        }

        public async Task<ApiResponseBase<DataValidatorExcel>> CreatePostFromExcelFileAsync(IBrowserFile file)
        {
            return await RequestClient.PostAPIWithFileAsync<ApiResponseBase<DataValidatorExcel>>("posts/import-excel", file);
        }

        public Task<ApiResponseBase<DataValidatorExcel>> CreatePostFromExcelFileAsync(IFormFile file)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponseBase<int>> IncreaseViewsTotalAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponseBase<List<string>>> GetListTags(int? skip, int? take)
        {
            throw new NotImplementedException();
        }
    }
}
