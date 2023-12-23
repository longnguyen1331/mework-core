using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using WebClient.Exceptions;

namespace WebClient.RequestHttp
{
    public static class RequestClient
    {
        private readonly static HttpClient _client;

        //intergated service
        public static IConfiguration Config;
        private static CancellationTokenSource _tokenSource;
        private static ILocalStorageService _localStorage;
        private static long UploadLimit = 25214400;
        static RequestClient()
        {
            _client = new HttpClient();
            _tokenSource = new CancellationTokenSource();
        }
        
        public static void CancelToken()
        {
            //ko được sử dụng cancel ở đây exception IO
        }

        public static void Initialize(IConfiguration configuration)
        {
            Config = configuration;
            _client.BaseAddress = new Uri(Config["RemoteServices:BaseUrl"]);
        }

        public static void InjectServices(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

     


        public static void AttachToken(string Token = "")
        {
            if (!string.IsNullOrEmpty(Token))
            {
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            }
        }


        public static async Task<T> GetAPIAsync<T>([Required] string URL)
        {
            try
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                httpResponseMessage = await _client.GetAsync(URL, _tokenSource.Token);
                return await ReturnApiResponse<T>(httpResponseMessage);
            }
            catch (Exception ex)
            {
                return await ReturnApiResponse<T>(null);
            }
        }
        
        public static async Task<T> PostAPIAsync<T>([Required] string URL, dynamic input, bool notifyOk = true)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();


            StringContent content =
                new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");

            httpResponseMessage = await _client.PostAsync(URL, content,_tokenSource.Token);

            return await ReturnApiResponse<T>(httpResponseMessage);
        }

        public static async Task<T> PatchAPIAsync<T>([Required] string URL, dynamic input, bool notifyOk = true)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();


            StringContent content =
                new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");

            httpResponseMessage = await _client.PatchAsync(URL, content,_tokenSource.Token);
            return await ReturnApiResponse<T>(httpResponseMessage);
        }

        public static async Task<T> PostAPIWithFileAsync<T>([Required] string URL, IBrowserFile file)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

            using (var ms = new MemoryStream())
            {
                await file.OpenReadStream(UploadLimit).CopyToAsync(ms);

                ms.Seek(0, SeekOrigin.Begin);
                using var content = new MultipartFormDataContent
                {
                    {new StreamContent(ms), "file", file.Name}
                };

                httpResponseMessage = await _client.PostAsync(URL, content,_tokenSource.Token);
                return await ReturnApiResponse<T>(httpResponseMessage);
            }
        }

        public static async Task<T> PostAPIWithMultipleFileAsync<T>([Required] string URL, List<IBrowserFile> files)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

            var streams = new List<MemoryStream>();

            var ms = new MemoryStream();
            using (var content = new MultipartFormDataContent())
            {
                foreach (var file in files)
                {
                    await file.OpenReadStream(UploadLimit).CopyToAsync(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    content.Add(new StreamContent(ms), $"files", file.Name);
                    streams.Add(ms);
                    ms = new MemoryStream();

                }

                httpResponseMessage = await _client.PostAsync(URL, content,_tokenSource.Token);
                foreach (var item in streams)
                {
                    item.Close();
                }

                return await ReturnApiResponse<T>(httpResponseMessage);
            }
        }


        public static async Task<T> PutAPIAsync<T>([Required] string URL, dynamic input)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            StringContent content =
                new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");
            httpResponseMessage = await _client.PutAsync(URL, content,_tokenSource.Token);
            return await ReturnApiResponse<T>(httpResponseMessage);
        }

        public static async Task<T> DeleteAPIAsync<T>([Required] string URL)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            httpResponseMessage = await _client.DeleteAsync(URL,_tokenSource.Token);
            return await ReturnApiResponse<T>(httpResponseMessage);
        }


        private static SemaphoreSlim Coordinator = new SemaphoreSlim(initialCount: 1, 1);
        private static bool IsWorking = true;
        private static async Task<T> ReturnApiResponse<T>(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string? jsonResponse = await httpResponseMessage.Content.ReadAsStringAsync() ?? null;
                return JsonConvert.DeserializeObject<T>(jsonResponse);
            }


            if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _localStorage.RemoveItemAsync("my-access-token");
                await _localStorage.RemoveItemAsync("my-refresh-token");
                throw new UnauthorizedException("");
            }

            if (httpResponseMessage.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new ServerErrorException("Server-Error");
            }

            if (httpResponseMessage.StatusCode == HttpStatusCode.TooManyRequests)
            {
                throw new TooManyRequests("Too many request");
            }

            if (httpResponseMessage.StatusCode == HttpStatusCode.Conflict)
            {
                string? jsonResponse = await httpResponseMessage.Content.ReadAsStringAsync() ?? null;
                var response = JsonConvert.DeserializeObject<ResponseApi>(jsonResponse);

                throw new ConflictException(response.message);
            }

            if (httpResponseMessage.StatusCode == HttpStatusCode.BadGateway)
            {
                throw new DbConnectionException("connection-error");
            }

            if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
            {
                string? jsonResponse = await httpResponseMessage.Content.ReadAsStringAsync() ?? null;
                var response = JsonConvert.DeserializeObject<ResponseApi>(jsonResponse);
                throw new BadRequestException(response.message);
            }

            return default;
        }
    }

    public class ResponseApi
    {
        public string message { get; set; }
        public int status { get; set; }
    }
}