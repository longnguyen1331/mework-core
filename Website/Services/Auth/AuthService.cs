using Contract;
using Contract.Identity.UserManager;
using Contract.Uploads;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Website.Models;
using Website.Models.ResponseModels;

namespace Website.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly RemoteAPIConfig _remoteOptions;

        private string CacheName = "Authenticate";

        public AuthService(IHttpClientFactory httpClientFactory,
                           IOptions<RemoteAPIConfig> remoteOptions,
                           IMemoryCache memoryCache)
        {
            if (remoteOptions is null)
            {
                throw new ArgumentNullException(nameof(remoteOptions));
            }

            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _remoteOptions = remoteOptions.Value;
        }

        public async Task<IDictionary<string, object>?> AuthAsync(UserModel input)
        {
            try
            {
                _memoryCache.TryGetValue(CacheName, out IDictionary<string, object> cacheData);

                DateTime expiredTime = DateTime.Now;

                if (cacheData == null)
                {
                    cacheData = new Dictionary<string, object>();

                    var client = _httpClientFactory.CreateClient("Website");
                    var response = await client.PostAsJsonAsync(_remoteOptions.Authenticate, input);

                    string? jsonResponse = await response.Content.ReadAsStringAsync() ?? null;

                    var accessToken = !string.IsNullOrEmpty(jsonResponse) ? JsonConvert.DeserializeObject<TokenDto>(jsonResponse) : null;

                    if (accessToken != null)
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var jwtSecurityToken = handler.ReadJwtToken(accessToken.AccessToken);

                        cacheData.Add("DeviceId", input.DeviceId ?? string.Empty);
                        cacheData.Add("AccessToken", accessToken.AccessToken ?? string.Empty);
                        cacheData.Add("FullName", jwtSecurityToken.Claims.First(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname").Value);
                        cacheData.Add("UserId", accessToken.UserId ?? jwtSecurityToken.Claims.First(claim => claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid").Value);
                        cacheData.Add("UserCode", jwtSecurityToken.Claims.First(claim => claim.Type == "Code").Value);
                        cacheData.Add("CompanyId", jwtSecurityToken.Claims.First(claim => claim.Type == "CompanyId").Value);

                        expiredTime = jwtSecurityToken.ValidTo;
                    }

                    _memoryCache.Set(CacheName, cacheData, expiredTime);
                }

                return cacheData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string? GetDeviceId()
        {
            _memoryCache.TryGetValue(CacheName, out IDictionary<string, object> cacheData);

            if (cacheData != null && cacheData.ContainsKey("DeviceId"))
                return cacheData["DeviceId"]?.ToString();

            return null;
        }

        public string? GetUserToken()
        {
            _memoryCache.TryGetValue(CacheName, out IDictionary<string, object> cacheData);

            if (cacheData != null && cacheData.ContainsKey("AccessToken"))
                return cacheData["AccessToken"]?.ToString();

            return null;
        }
        

        public string? GetUserCode()
        {
            _memoryCache.TryGetValue(CacheName, out IDictionary<string, object> cacheData);

            if (cacheData != null && cacheData.ContainsKey("UserCode"))
                return cacheData["UserCode"]?.ToString();

            return null;
        }

        public string? GetUserId()
        {
            _memoryCache.TryGetValue(CacheName, out IDictionary<string, object> cacheData);

            if (cacheData != null && cacheData.ContainsKey("UserId"))
                return cacheData["UserId"]?.ToString();

            return null;
        }

        public async Task SignOutAsync(WebisteUserLogOutModel token)
        {
            _memoryCache.Remove(CacheName);
            var client = _httpClientFactory.CreateClient("Website");
            var response = await client.PostAsJsonAsync("user/sign-out-website", token);
            
        }

        public async Task<ApiResponseBase<UserDto>?> SignUpAsync(CreateUserDto input)
        {
            var client = _httpClientFactory.CreateClient("Website");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _remoteOptions.CreatePatient);
            request.Content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();

            result.EnsureSuccessStatusCode();

            var patientResult = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<ApiResponseBase<UserDto>>(response) : null;

            await AuthAsync(new UserModel() { UserName = input.UserName, Password = input.Password });

            return patientResult;
        }

        public async Task<ApiResponseBase<UserDto>?> UpdateAsync(UpdateUserModel input, Guid id)
        {
            var client = _httpClientFactory.CreateClient("Website");

            var auth = await AuthAsync(new UserModel());

            // Get latest patient ìnfo
            var request = new HttpRequestMessage(HttpMethod.Get, string.Format(_remoteOptions.GetPatient, id));

            var result = await client.SendAsync(request);
            var response = await result.Content.ReadAsStringAsync();

            var patientResult = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<ApiResponseBase<UserWithNavigationPropertiesDto>>(response) : null;

            if (patientResult != null)
            {
                string[] dateFormat = new string[] { "dd/MM/yyyy", "d/M/yyyy", "dd/MM/yy", "d/M/yy" };
                DateTime.TryParseExact(input.DOB, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime dob);

                UpdateUserDto updateRequest = new UpdateUserDto()
                {
                    Address = input.IsOnLyUpdateAvatar ? patientResult.Data.User.Address : input.Address,
                    FirstName = input.IsOnLyUpdateAvatar ? patientResult.Data.User.FirstName : input.FirstName,
                    AvatarURL = input.IsOnLyUpdateAvatar ? input.AvatarURL : patientResult.Data.User.AvatarURL,
                    BloodTypeId = patientResult.Data.User.BloodTypeId,
                    CitizenIDNumber = input.IsOnLyUpdateAvatar ? patientResult.Data.User.CitizenIDNumber : input.CitizenIDNumber,
                    CountryId = patientResult.Data.User.CountryId,
                    CreatedBy = patientResult.Data.User.CreatedBy,
                    DistrictId = input.IsOnLyUpdateAvatar ? patientResult.Data.User.DistrictId : input.DistrictId,
                    DOB = input.IsOnLyUpdateAvatar ? patientResult.Data.User.DOB : dob,
                    Email = input.IsOnLyUpdateAvatar ? patientResult.Data.User.Email : input.Email,
                    Gender = input.IsOnLyUpdateAvatar ? patientResult.Data.User.Gender : input.Gender,
                    LastName = input.IsOnLyUpdateAvatar ? patientResult.Data.User.LastName : input.LastName,
                    ProvinceId = input.IsOnLyUpdateAvatar ? patientResult.Data.User.ProvinceId : input.ProvinceId,
                    WardId = input.IsOnLyUpdateAvatar ? patientResult.Data.User.WardId : input.WardId,
                    UserName = patientResult.Data.User.UserName,
                    DependsId = patientResult.Data.User.DependsId,
                    IsActive = patientResult.Data.User.IsActive,
                    IsSetPassword = false,
                    PhoneNumber = patientResult.Data.User.PhoneNumber,
                    UserCode = patientResult.Data.User.UserCode,
                    ODX = patientResult.Data.User.ODX ?? 0,
                    Relationship = patientResult.Data.User.Relationship,
                    ModifiedBy = id
                };

                request = new HttpRequestMessage(HttpMethod.Put, string.Format(_remoteOptions.UpdatePatient, id));
                request.Content = new StringContent(JsonConvert.SerializeObject(updateRequest), Encoding.UTF8, "application/json");

                if (auth != null && auth.ContainsKey("AccessToken"))
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth["AccessToken"]);

                result = await client.SendAsync(request);
                response = await result.Content.ReadAsStringAsync();

                if (!result.IsSuccessStatusCode)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponseModel>(response);
                    if (error != null)
                        return new ApiResponseBase<UserDto>() { Message = error.Message, StatusCode = error.Status };
                }

                result.EnsureSuccessStatusCode();

                var updateResult = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<ApiResponseBase<UserDto>>(response) : null;

                return updateResult;
            }
            return null;
        }
        public async Task<FileDto?> UploadImage(IFormFile file)
        {
            var client = _httpClientFactory.CreateClient("Website");

            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

            var streams = new List<MemoryStream>();

            var ms = new MemoryStream();
            using (var content = new MultipartFormDataContent())
            {
                await file.OpenReadStream().CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                content.Add(new StreamContent(ms), $"file", file.FileName);
                streams.Add(ms);
                ms = new MemoryStream();

                httpResponseMessage = await client.PostAsync(_remoteOptions.UploadFile, content);
                foreach (var item in streams)
                {
                    item.Close();
                }

                var response = await httpResponseMessage.Content.ReadAsStringAsync();

                var fileResult = !string.IsNullOrEmpty(response) ? JsonConvert.DeserializeObject<FileDto>(response) : null;

                return fileResult;
            }
        }
    }
}
