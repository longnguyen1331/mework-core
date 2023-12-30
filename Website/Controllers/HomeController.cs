using Contract;

using Contract.PostCategories;
using Contract.Posts;
using Contract.Services;
using Contract.ServiceTypes;

using Contract.WebBanners;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using Website.Models;
using Website.Models.ResponseModels;
using Website.Utils;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RemoteAPIConfig _remoteOptions;
        private readonly NewsInHomepageConfig _newsInHomepageConfigOptions;

        public HomeController(ILogger<HomeController> logger,
                              IHttpClientFactory httpClientFactory,
                              IOptions<RemoteAPIConfig> remoteOptions,
                              IOptions<NewsInHomepageConfig> newsInHomepageConfigOptions)
        {
            if (remoteOptions is null)
            {
                throw new ArgumentNullException(nameof(remoteOptions));
            }

            if (newsInHomepageConfigOptions is null)
            {
                throw new ArgumentNullException(nameof(newsInHomepageConfigOptions));
            }

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _remoteOptions = remoteOptions.Value;
            _newsInHomepageConfigOptions = newsInHomepageConfigOptions.Value;
        }
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("Website");

            #region Get banner
            var response = await client.GetStringAsync(_remoteOptions.GetWebBanner);

            var bannerCacheData = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<List<WebBannerDto>>(response) : null;
            ViewData["Banners"] = bannerCacheData;
            #endregion


            ViewData["HealthInfomationIds"] = _newsInHomepageConfigOptions.HealthInformation;
            ViewData["NewsCategoryIds"] = _newsInHomepageConfigOptions.NewsCategory;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, StatusCode = HttpContext.Response.StatusCode.ToString() });
        }


        [HttpGet]
        public async Task<JsonResult> GetTongDanGiaSuc()
        {
            var client = _httpClientFactory.CreateClient("Statics");
            var response = await client.GetStringAsync(_remoteOptions.TongDanGiaSuc);
            var dataTongDan = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<ChartResponseModel>(response) : null;
            return Json(dataTongDan);
        }
        [HttpGet]
        public async Task<JsonResult> GetTongDanGiaCam()
        {
            var client = _httpClientFactory.CreateClient("Statics");
            var response = await client.GetStringAsync(_remoteOptions.TongDanGiaCam);
            var dataTongDan = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<ChartResponseModel>(response) : null;
            return Json(dataTongDan);
        }

        [HttpPost]
        public async Task<JsonResult> GetPosts([FromBody] PostFitlerPagingDto filter)
        {
            var client = _httpClientFactory.CreateClient("Website");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _remoteOptions.GetPosts);
            request.Content = new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json");

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();

            var postResult = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<ApiResponseBase<PostSearchResultDto>>(response) : null;

            return Json(postResult?.Data?.Items);
        }

        [HttpPost]
        public async Task<JsonResult> GetServices([FromBody] ServiceFilterPagingDto filter)
        {
            var client = _httpClientFactory.CreateClient("Website");

            if (filter == null)
                filter = new ServiceFilterPagingDto()
                {
                    Take = 10,
                    IsHighLight = true
                };
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _remoteOptions.GetServiceAdvanceFilter);
            request.Content = new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json");

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();

            var highlightServices = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<ApiResponseBase<ServiceSearchResultDto>>(response) : null;

            return Json(highlightServices?.Data?.Items);
        }

        //[HttpPost]
        //public async Task<JsonResult> GetDoctors()
        //{
        //    var client = _httpClientFactory.CreateClient("Website");

        //    var response = await client.GetStringAsync(_remoteOptions.GetTopRateDoctor);
        //    var doctors = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<List<DoctorInfoWithNavigationDto>>(response) : null;

        //    return Json(doctors);
        //}

        //[HttpPost]
        //public async Task<JsonResult> GetSpecialties()
        //{
        //    var client = _httpClientFactory.CreateClient("Website");

        //    var response = await client.GetStringAsync(_remoteOptions.GetSpecialties);
        //    var specialties = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<List<SpecialtyDto>>(response) : null;
        //    return Json(specialties);
        //}

        [HttpPost]
        public async Task<JsonResult> GetServiceTypes([FromBody] ServiceTypeFilterPagingDto filter)
        {
            var client = _httpClientFactory.CreateClient("Website");

            if (filter == null)
                filter = new ServiceTypeFilterPagingDto()
                {
                    Take = 8,
                    IsHighlight = true
                };
            var request = new HttpRequestMessage(HttpMethod.Post, _remoteOptions.GetServiceTypes);
            request.Content = new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json");

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();

            var serviceTypes = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<List<ServiceTypeDto>>(response) : null;

            return Json(serviceTypes);
        }

        [HttpPost]
        public async Task<JsonResult> GetPostCatefories([FromBody] PostCategoryFitlerPagingDto filter)
        {
            var client = _httpClientFactory.CreateClient("Website");

            if (filter == null)
                filter = new PostCategoryFitlerPagingDto()
                {
                    Ids = _newsInHomepageConfigOptions.NewsCategory,
                    Take = 10
                };
            var request = new HttpRequestMessage(HttpMethod.Post, _remoteOptions.GetPostCategories);
            request.Content = new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json");

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();

            var postCategories = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<List<PostCategoryDto>>(response)?.OrderBy(x => x.ODX).ToList() : null;

            return Json(postCategories);
        }
    }
}