using System.Net;

namespace Website.Models.ResponseModels
{
    public class ErrorResponseModel
    {
        public string Message { get; set; } = string.Empty;
        public HttpStatusCode Status { get; set; }
    }
}
