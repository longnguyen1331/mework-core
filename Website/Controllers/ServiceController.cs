using Contract;


using Contract.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using Website.Models;
using Website.Services.Auth;

namespace Website.Controllers
{
    [Route("service/")]
    public class ServiceController : Controller
    {
        private readonly ILogger<ServiceController> _logger;
		private readonly IAuthService _authService;
		private readonly IHttpClientFactory _httpClientFactory;
        private readonly UrlConfig _urlConfig;
        private readonly RemoteAPIConfig _remoteOptions;

        public ServiceController(ILogger<ServiceController> logger, IAuthService authService, IHttpClientFactory httpClientFactory, IOptions<RemoteAPIConfig> remoteOptions, IOptions<UrlConfig> urlConfig)
        {
            _logger = logger;
			_authService = authService;
			_httpClientFactory = httpClientFactory;
            _urlConfig = urlConfig.Value;
            _remoteOptions = remoteOptions.Value;
        }
        [Route("")]
        [Route("{slug}_p_{id}")]
        public async Task<IActionResult> Index(string? slug, string? id, string? keyword)
        {
            var client = _httpClientFactory.CreateClient("Website");

            #region Get top 10 doctor
            var response = await client.GetStringAsync(_remoteOptions.GetTopRateDoctor);
            //ViewData["TopDoctor"] = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<List<DoctorInfoWithNavigationDto>>(response) : null;
            #endregion

            ViewData["ServiceTypeId"] = id;
            ViewData["Keyword"] = keyword;

            return View();
        }

        [Route("/servicedetail/{slug}_pdt_{id}")]
        public async Task<IActionResult> ServiceDetail(string? slug, string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("goi-dich-vu");
            }

            var client = _httpClientFactory.CreateClient("Website");

            #region Update views
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, string.Format(_remoteOptions.IncreaseServiceViews, id));
            await client.SendAsync(request);
            #endregion

            #region Get service by id
            request = new HttpRequestMessage(HttpMethod.Get, string.Format(_remoteOptions.GetServiceById, id));

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();

            var serviceResult = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<ApiResponseBase<ServiceDto>>(response) : null;

            var service = serviceResult?.Data;
            ViewData["Service"] = service;
            #endregion

            #region Get related
            request = new HttpRequestMessage(HttpMethod.Post, _remoteOptions.GetServiceAdvanceFilter);
            var filter = new ServiceFilterPagingDto()
            {
                Take = 10,
                ServiceTypeId = service?.ServiceTypeId,
                IgnoreServiceId = service?.Id
            };

            request.Content = new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json");

            result = await client.SendAsync(request);
            response = await result.Content.ReadAsStringAsync();

            var serviceResults = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<ApiResponseBase<ServiceSearchResultDto>>(response) : null;

            ViewData["ServiceRelated"] = serviceResults?.Data?.Items;
            ViewData["GetAdviceUrl"] = _urlConfig.GetAdviceUrl;
            #endregion

            return View();
        }

        [HttpPost]
        [Route("getServicesPanel")]
        public async Task<IActionResult> GetServicesPanel([FromBody] ServiceFilterPagingDto filter)
        {
            var viewData = await  GetListServices(filter);

            return PartialView("_ServiceResult", viewData);
        }

        [HttpPost]
        [Route("getServicesPanelForBookingAppointment")]
        public async Task<IActionResult> GetServicesPanelForBookingAppointment([FromBody] ServiceFilterPagingDto filter)
        {
            var viewData = await GetListServices(filter);

            viewData["ServiceKeyword"] = filter.FilterText;
			ViewData["UserId"] = _authService.GetUserId();

			return PartialView("_BookingServiceResult", viewData);
        }
        
        [HttpGet]
        [Route("getServicesDetailForBookingAppointment/{serviceId}")]
        public async Task<IActionResult> GetServicesDetailForBookingAppointment(string? serviceId)
        {
            var client = _httpClientFactory.CreateClient("Website");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Format(_remoteOptions.GetServiceById, serviceId));

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();

            var serviceResult = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<ApiResponseBase<ServiceDto>>(response) : null;

            var service = serviceResult?.Data;
            ViewData["Service"] = service;
			ViewData["UserId"] = _authService.GetUserId();

            return PartialView("_BookingServiceDetailResult", ViewData);
        }

        //[HttpGet]
        //[Route("getDoctorByServiceId/{serviceId}")]
        //public async Task<IActionResult> GetDoctorsByServiceId(Guid serviceId)
        //{
        //    var client = _httpClientFactory.CreateClient("Website");

        //    var request = new HttpRequestMessage(HttpMethod.Get, string.Format(_remoteOptions.GetDoctorByServiceId, serviceId));

        //    var result = await client.SendAsync(request);
        //    var response = await result.Content.ReadAsStringAsync();

        //    return Json(!string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<List<DoctorServiceDto>>(response) : null);
        //}

        //[HttpGet]
        //[Route("getDoctorWorkingHour/{doctorId}")]
        //public async Task<IActionResult> GetDoctorWorkingHour(Guid doctorId)
        //{
        //    var client = _httpClientFactory.CreateClient("Website");

        //    var request = new HttpRequestMessage(HttpMethod.Get, string.Format(_remoteOptions.GetDoctorWorkingHour, doctorId));

        //    var result = await client.SendAsync(request);
        //    var response = await result.Content.ReadAsStringAsync();

        //    return Json(!string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<List<DoctorWorkHourDto>>(response) : null);
        //}

        private async Task<ViewDataDictionary> GetListServices(ServiceFilterPagingDto filter)
        {
            var client = _httpClientFactory.CreateClient("Website");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _remoteOptions.GetServiceAdvanceFilter);
            request.Content = new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json");

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();

            var postResult = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<ApiResponseBase<ServiceSearchResultDto>>(response) : null;

            ViewData["ServiceResult"] = postResult?.Data;
            ViewData["CurPage"] = (int)((decimal)filter.Skip / (filter.Take != 0 ? filter.Take : 1)) + 1;

            var totalPage = (int)Math.Ceiling((decimal)(postResult?.Data?.TotalItems ?? 0) / (filter.Take != 0 ? filter.Take : 1));
            ViewData["TotalPage"] = totalPage;
            ViewData["Pages"] = Enumerable.Range(1, totalPage).ToList();

            return ViewData;
        }
    }
}