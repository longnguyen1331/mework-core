using Application.Posts;
using Contract;
using Contract.Common.Excels;
using Contract.Posts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/posts/")]
    [Authorize]
    public class PostController(PostService _postService)
    {
        [HttpPost]
        public async Task<ApiResponseBase<PostDto>> CreateAsync(CreateUpdatePostDto input)
        {
            return await _postService.CreateAsync(input);
        }

        [HttpDelete]
		[Route("{id}")]
        public async Task<ApiResponseBase<PostDto>> DeleteAsync(Guid id)
        {
            return await _postService.DeleteAsync(id);
        }

        [HttpGet]
        public async Task<ApiResponseBase<List<PostDto>>> GetListAsync()
        {
            return await _postService.GetListAsync();
        }
        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<ApiResponseBase<PostDto>> GetByIdAsync(Guid id)
        {
            return await _postService.GetByIdAsync(id);
        }

        [HttpPut]
		[Route("{id}")]
        public async Task<ApiResponseBase<PostDto>> UpdateAsync(CreateUpdatePostDto input, Guid id)
        {
            return await _postService.UpdateAsync(input, id);
        }

        [HttpPost]
		[Route("filters")]
        public async Task<ApiResponseBase<List<PostDto>>> GetListPagingAsync(BaseFilterPagingDto filter)
        {
            return await _postService.GetListPagingAsync(filter);
        }

        [HttpPost]
		[Route("advance-filters")]
        [AllowAnonymous]
        public async Task<ApiResponseBase<PostSearchResultDto>> GetListPagingAsync(PostFitlerPagingDto filter)
        {
            return await _postService.GetListPagingAsync(filter);
        }

        [HttpGet]
		[Route("related-post/{id}")]
        [AllowAnonymous]
        public async Task<ApiResponseBase<List<PostDto>>> GetRelatedListAsync(Guid id)
        {
            return await _postService.GetRelatedListAsync(id);
        }

        [HttpPost]
		[Route("export-excel")]
        public async Task<ApiResponseBase<byte[]>> ExportExcel(PostFitlerPagingDto filter)
        {
            return await _postService.ExportExcel(filter);
        }

        [HttpPost]
		[Route("import-excel")]
        public async Task<ApiResponseBase<DataValidatorExcel>> CreatePostFromExcelFileAsync([FromForm] IFormFile file)
        {
            return await _postService.CreatePostFromExcelFileAsync(file);
        }

        [HttpPut]
		[Route("increase-views/{id}")]
        [AllowAnonymous]
        public async Task<ApiResponseBase<int>> IncreaseViewsTotalAsync(Guid id)
        {
            return await _postService.IncreaseViewsTotalAsync(id);
        }

        [HttpGet]
		[Route("tags")]
        [AllowAnonymous]
        public async Task<ApiResponseBase<List<string>>> GetListTags(int? skip, int? take)
        {
            return await _postService.GetListTags(skip ?? 0, take ?? 15);
        }
    }
}
