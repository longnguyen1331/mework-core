using Contract;

using Contract.PostCategories;
using Contract.Posts;
using Contract.Services;
using Contract.ServiceTypes;

using Contract.WebBanners;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Website.Models;
using Website.Models.RequestModels;
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
        public async Task<JsonResult> GetStatics([FromQuery] 
            int? idTinhThanh, int? idQuanHuyen, int? idXaPhuong,
            string? fromDate, string? toDate
            )
        {
            string paramsGetQuery = string.Empty;
            try
            {
                if (idTinhThanh.HasValue) paramsGetQuery += $"idTinhThanh={idTinhThanh}";
                if (idQuanHuyen.HasValue) paramsGetQuery += (!string.IsNullOrEmpty(paramsGetQuery) ? "&" : "") + $"idQuanHuyen={idQuanHuyen}";
                if (idQuanHuyen.HasValue) paramsGetQuery += (!string.IsNullOrEmpty(paramsGetQuery) ? "&" : "") + $"idXaPhuong={idXaPhuong}";
                if (fromDate != null && !string.IsNullOrEmpty(fromDate)) paramsGetQuery += (!string.IsNullOrEmpty(paramsGetQuery) ? "&" : "") + $"fromDate={fromDate}";
                if (toDate != null && !string.IsNullOrEmpty(toDate)) paramsGetQuery += (!string.IsNullOrEmpty(paramsGetQuery) ? "&" : "") +  $"toDate={toDate}";



                var client = _httpClientFactory.CreateClient("Statics");

                var resApiTongDanGiaSuc = client.GetStringAsync(_remoteOptions.TongDanGiaSuc + paramsGetQuery);
                var resApiTongDanGiaCam = client.GetStringAsync(_remoteOptions.TongDanGiaCam + paramsGetQuery);
                var resApiSanLuongThitGiaSuc = client.GetStringAsync(_remoteOptions.SanLuongThitGiaSuc + paramsGetQuery);
                var resApiSanLuongThitGiaCam = client.GetStringAsync(_remoteOptions.SanLuongThitGiaCam + paramsGetQuery);
                var resApiSanLuongTrung = client.GetStringAsync(_remoteOptions.SanLuongTrung + paramsGetQuery);
                var resApiSanLuongSua = client.GetStringAsync(_remoteOptions.SanLuongSua + paramsGetQuery);
                var resApiSanLuongSanXuatThucAn = client.GetStringAsync(_remoteOptions.SanLuongSanXuatThucAn + paramsGetQuery);
                var resApiSanLuongTieuThuThucAn = client.GetStringAsync(_remoteOptions.SanLuongTieuThuThucAn + paramsGetQuery);
                var resApiDichBenh = client.GetStringAsync(_remoteOptions.DichBenh + paramsGetQuery);
                var resApiTiemPhong = client.GetStringAsync(_remoteOptions.TiemPhong + paramsGetQuery);
                var resApiThongKe = client.GetStringAsync(_remoteOptions.ThongKe + paramsGetQuery);
                var resApiThongKeChanNuoi = client.GetStringAsync(_remoteOptions.ThongKeChanNuoi + paramsGetQuery);

                // Wait for both tasks to complete.
                await Task.WhenAll(
                    resApiTongDanGiaSuc,
                    resApiTongDanGiaCam,
                    resApiSanLuongThitGiaSuc,
                    resApiSanLuongThitGiaCam,
                    resApiSanLuongTrung,
                    resApiSanLuongSua,
                    resApiSanLuongSanXuatThucAn,
                    resApiSanLuongTieuThuThucAn,
                    resApiDichBenh,
                    resApiTiemPhong,
                    resApiThongKe,
                    resApiThongKeChanNuoi
                );

                var resApiThongKeResult = !string.IsNullOrEmpty(resApiThongKe.Result) ? JsonConvert.DeserializeObject<ThongKeResponseModel>(resApiThongKe.Result) : null;
                ChartResponseModel dataThongKe  = new ChartResponseModel();
                PropertyInfo[] properties = typeof(ThongKeResponseModel).GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    dataThongKe.Labels.Add(property.Name);
                    dataThongKe.Data.Add((int)property.GetValue(resApiThongKeResult));
                }

                var data = new
                {
                    dataTongDanGiaSuc = !string.IsNullOrEmpty(resApiTongDanGiaSuc.Result) ? JsonConvert.DeserializeObject<ChartResponseModel>(resApiTongDanGiaSuc.Result) : null,
                    dataTongDanGiaCam = !string.IsNullOrEmpty(resApiTongDanGiaCam.Result) ? JsonConvert.DeserializeObject<ChartResponseModel>(resApiTongDanGiaCam.Result) : null,
                    dataSanLuongThitGiaSuc = !string.IsNullOrEmpty(resApiSanLuongThitGiaSuc.Result) ? JsonConvert.DeserializeObject<ChartResponseModel>(resApiSanLuongThitGiaSuc.Result) : null,
                    dataSanLuongThitGiaCam = !string.IsNullOrEmpty(resApiSanLuongThitGiaCam.Result) ? JsonConvert.DeserializeObject<ChartResponseModel>(resApiSanLuongThitGiaCam.Result) : null,
                    dataSanLuongTrung = !string.IsNullOrEmpty(resApiSanLuongTrung.Result) ? JsonConvert.DeserializeObject<ChartResponseModel>(resApiSanLuongTrung.Result) : null,
                    dataSanLuongSua = !string.IsNullOrEmpty(resApiSanLuongSua.Result) ? JsonConvert.DeserializeObject<ChartResponseModel>(resApiSanLuongSua.Result) : null,
                    dataSanLuongSanXuatThucAn = !string.IsNullOrEmpty(resApiSanLuongSanXuatThucAn.Result) ? JsonConvert.DeserializeObject<ChartResponseModel>(resApiSanLuongSanXuatThucAn.Result) : null,
                    dataSanLuongTieuThuThucAn = !string.IsNullOrEmpty(resApiSanLuongTieuThuThucAn.Result) ? JsonConvert.DeserializeObject<ChartResponseModel>(resApiSanLuongTieuThuThucAn.Result) : null,
                    dataDichBenh = !string.IsNullOrEmpty(resApiDichBenh.Result) ? JsonConvert.DeserializeObject<ChartResponseModel>(resApiDichBenh.Result) : null,
                    dataTiemPhong = !string.IsNullOrEmpty(resApiTiemPhong.Result) ? JsonConvert.DeserializeObject<ChartResponseModel>(resApiTiemPhong.Result) : null,
                    dataThongKe = dataThongKe,
                    objectThongKe = resApiThongKeResult,
                    dataThongKeChanNuoi = !string.IsNullOrEmpty(resApiThongKeChanNuoi.Result) ? JsonConvert.DeserializeObject<List<ThongKeChanNuoiChiTietResponseModel>>(resApiThongKeChanNuoi.Result) : null,
                };
                return Json(data);
            }
            catch (Exception ex) {

                return Json(null);

            }

        }

        [HttpGet]
        public async Task<JsonResult> GetProvinces()
        {
            var client = _httpClientFactory.CreateClient("Statics");
            var response = await client.GetStringAsync(_remoteOptions.Tinh);
            var data = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<List<BaseModel>>(response) : null;
            data.FirstOrDefault(x => x.Id == 51).Selected = true;
            data.FirstOrDefault(x => x.Id == 51).Disabled = false;
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetDistricts([FromQuery]int? provinceId)
        {
            List<BaseModel> result = new List<BaseModel>();
            if(provinceId != null && provinceId.Value > 0)
            {
                var client = _httpClientFactory.CreateClient("Statics");
                var response = await client.GetStringAsync($"{_remoteOptions.Huyen}/{provinceId.Value}");
                result = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<List<BaseModel>>(response) : null;

                result.Insert(0, new BaseModel { Id = 0, Ten = "Tất cả", Selected = true });
            }
            
            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetWards([FromQuery] int? districtId)
        {
            List<BaseModel> result = new List<BaseModel>();
            if (districtId != null && districtId.Value > 0)
            {
                var client = _httpClientFactory.CreateClient("Statics");
                var response = await client.GetStringAsync($"{_remoteOptions.Xa}/{districtId.Value}");
                result = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<List<BaseModel>>(response) : null;
                result.Insert(0, new BaseModel { Id = 0, Ten = "Tất cả", Selected = true });
            }

            return Json(result);
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