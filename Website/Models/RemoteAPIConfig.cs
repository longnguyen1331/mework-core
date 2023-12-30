namespace Website.Models
{
    public class RemoteAPIConfig
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string StaticsApiUrl { get; set; } = string.Empty;
        public string TongDanGiaSuc { get; set; } = string.Empty;
        public string TongDanGiaCam { get; set; } = string.Empty;

        public string GetAppConfig { get; set; } = string.Empty;
        public string GetWebBanner { get; set; } = string.Empty;
        public string GetWebsiteMenu { get; set; } = string.Empty;
        public string GetServiceAdvanceFilter { get; set; } = string.Empty;
        public string GetServiceById { get; set; } = string.Empty;
        public string GetTopRateDoctor { get; set; } = string.Empty;
        public string GetSpecialties { get; set; } = string.Empty;
        public string GetServiceTypes { get; set; } = string.Empty;
        public string GetPostCategories { get; set; } = string.Empty;
        public string GetPosts { get; set; } = string.Empty;
        public string GetPostById { get; set; } = string.Empty;
        public string GetTagsOfPost { get; set; } = string.Empty;
        public string GetDoctorById { get; set; } = string.Empty;
        public string GetDoctorAdvanceFilter { get; set; } = string.Empty;
        public string GetDoctorByServiceId { get; set; } = string.Empty;
        public string GetDoctorWorkingHour { get; set; } = string.Empty;
        public string Authenticate { get; set; } = string.Empty;
        public string CreatePatient { get; set; } = string.Empty;
        public string UpdatePatient { get; set; } = string.Empty;
        public string GetPatient { get; set; } = string.Empty;
        public string GetProvince { get; set; } = string.Empty;
        public string GetDistrictByProvinceId { get; set; } = string.Empty;
        public string GetWardByDistrictId { get; set; } = string.Empty;
        public string CreateAppointment { get; set; } = string.Empty;

        public string IncreasePostViews { get; set; } = string.Empty;
        public string IncreaseServiceViews { get; set; } = string.Empty;

        public string UploadFile { get; set; } = string.Empty;

        public string BaseWebviewUrl { get; set; } = string.Empty;
        public string WebviewUrl { get; set; } = string.Empty;
    }
}
