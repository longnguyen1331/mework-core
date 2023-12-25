using Contract;

using Contract.Posts;
using Contract.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using Website.Models;

namespace Website.Controllers
{
    [Route("postCategory/")]
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RemoteAPIConfig _remoteOptions;

        public PostController(ILogger<PostController> logger, IHttpClientFactory httpClientFactory, IOptions<RemoteAPIConfig> remoteOptions)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _remoteOptions = remoteOptions.Value;
        }
        [Route("")]
        [Route("tags")]
        [Route("{slug}_n_{id}")]
        public async Task<IActionResult> Index(string? slug, string? id, string? keyword, string? tag)
        {
            if (HttpContext.Request.Path.Value?.Contains("/tags") == true && string.IsNullOrEmpty(tag))
                return Redirect("/postCategory");

            var client = _httpClientFactory.CreateClient("Website");

            //#region Get top 10 doctor
            //var response = await client.GetStringAsync(_remoteOptions.GetTopRateDoctor);
            //ViewData["TopDoctor"] = "" ;
            //#endregion
            #region Get highlight service
            var request = new HttpRequestMessage(HttpMethod.Post, _remoteOptions.GetServiceAdvanceFilter);
            ServiceFilterPagingDto filter = new ServiceFilterPagingDto()
            {
                Take = 10,
                IsHighLight = true
            };

            request.Content = new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json");

            var result = await client.SendAsync(request);
           var   response = await result.Content.ReadAsStringAsync();

            var highlightServices = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<ApiResponseBase<ServiceSearchResultDto>>(response) : null;

            ViewData["HightlightServices"] = highlightServices?.Data?.Items;
            #endregion

            ViewData["Keyword"] = keyword;
            ViewData["Tag"] = tag;
            ViewData["IsShowSearch"] = (string.IsNullOrEmpty(id) || !string.IsNullOrEmpty(keyword)) && string.IsNullOrEmpty(tag);
            ViewData["PostCategoryId"] = id;
            
            return View();
        }
        
        [Route("/post/{slug}_dt_{id}")]
        [Route("/post/{slug}_a_{id}")]
        public async Task<IActionResult> PostDetail(string? slug, string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("tin-tuc");
            }

            var client = _httpClientFactory.CreateClient("Website");

            #region Get top 10 doctor
           // var response = await client.GetStringAsync(_remoteOptions.GetTopRateDoctor);
            //ViewData["TopDoctor"] = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<List<DoctorInfoWithNavigationDto>>(response) : null;
            #endregion

            #region Get highlight service
            var request = new HttpRequestMessage(HttpMethod.Post, _remoteOptions.GetServiceAdvanceFilter);
            ServiceFilterPagingDto serviceFilter = new ServiceFilterPagingDto()
            {
                Take = 10,
                IsHighLight = true
            };

            request.Content = new StringContent(JsonConvert.SerializeObject(serviceFilter), Encoding.UTF8, "application/json");

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();

            var highlightServices = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<ApiResponseBase<ServiceSearchResultDto>>(response) : null;

            ViewData["HightlightServices"] = highlightServices?.Data?.Items;
            #endregion

            #region Update views
            request = new HttpRequestMessage(HttpMethod.Put, string.Format(_remoteOptions.IncreasePostViews, id));
            await client.SendAsync(request);
            #endregion
            
            #region Get Post by id
            request = new HttpRequestMessage(HttpMethod.Get, string.Format(_remoteOptions.GetPostById, id));

            result = await client.SendAsync(request);
            response = await result.Content.ReadAsStringAsync();

            var postResult = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<ApiResponseBase<PostDto>>(response) : null;

            var post = postResult?.Data;
            ViewData["Post"] = post;
            #endregion

            bool isIntro = HttpContext.Request.Path.Value?.Contains("gioi-thieu") ?? false;
            ViewData["IsIntro"] = isIntro;

            if (!isIntro)
            {
                #region Get related
                request = new HttpRequestMessage(HttpMethod.Post, _remoteOptions.GetPosts);
                var postFilter = new PostFitlerPagingDto()
                {
                    Take = 10,
                    PostCategoryId = post?.PostCategoryId,
                    IgnorePostId = post?.Id
                };

                request.Content = new StringContent(JsonConvert.SerializeObject(postFilter), Encoding.UTF8, "application/json");

                result = await client.SendAsync(request);
                response = await result.Content.ReadAsStringAsync();

                var postResults = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<ApiResponseBase<PostSearchResultDto>>(response) : null;

                ViewData["PostRelated"] = postResults?.Data?.Items;
                ViewData["Image"] = post.PictureUrl;
                if (post.SeoTitle == null) { ViewData["Title"] = post.Title; } else ViewData["Title"] = post.SeoKeyword;
                
                if (post.SeoKeyword == null) { ViewData["SEOKeyword"] = post.Tags; } else ViewData["SEOKeyword"] = post.SeoKeyword;
               
                if(post.SeoDescription == null) { ViewData["SEODescription"] = post.SortDescription; } else ViewData["SEODescription"] = post.SeoDescription;

                #endregion
            }

            return View();
        }

        [HttpPost]
        [Route("getPostsPanel")]
        public async Task<IActionResult> GetPostsPanel([FromBody]PostFitlerPagingDto filter)
        {
            var client = _httpClientFactory.CreateClient("Website");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _remoteOptions.GetPosts);
            request.Content = new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json");

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();

            var postResult = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<ApiResponseBase<PostSearchResultDto>>(response) : null;

            ViewData["PostResult"] = postResult?.Data;
            ViewData["CurPage"] = (int)((decimal)filter.Skip / (filter.Take != 0 ? filter.Take : 1)) + 1;
            
            var totalPage = (int)Math.Ceiling((decimal)(postResult?.Data?.TotalItems ?? 0) / (filter.Take != 0 ? filter.Take : 1));
            ViewData["TotalPage"] = totalPage;
            ViewData["Pages"] = Enumerable.Range(1, totalPage).ToList();

            return PartialView("_PostResult", ViewData);
        }
        
        [HttpPost]
        [Route("getPostsLeftPanel")]
        public async Task<IActionResult> GetPostsLeftPanel([FromBody]PostFitlerPagingDto filter)
        {
            var client = _httpClientFactory.CreateClient("Website");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _remoteOptions.GetPosts);
            request.Content = new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json");

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();

            var postResult = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<ApiResponseBase<PostSearchResultDto>>(response) : null;
            ViewData["PostResult"] = postResult?.Data;

            return PartialView("_HotPostResult", ViewData);
        }

        [HttpGet]
        [Route("getTagsPanel")]
        public async Task<IActionResult> GetTagsOfPosts(int? skip, int? take)
        {
            var client = _httpClientFactory.CreateClient("Website");

            string queryString = string.Empty;

            if (skip.HasValue)
                queryString += $"{(!string.IsNullOrEmpty(queryString) ? "&" : "?")}skip={skip}";
            
            if (take.HasValue)
                queryString += $"{(!string.IsNullOrEmpty(queryString) ? "&" : "?")}take={take}";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{_remoteOptions.GetTagsOfPost}{queryString}");

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();

            var tagResult = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<ApiResponseBase<List<string>>>(response) : null;

            ViewData["Tags"] = tagResult?.Data;

            return PartialView("_TagsOfPostResult", ViewData);
        }
    }
}