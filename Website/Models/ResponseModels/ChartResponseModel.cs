using System.Net;

namespace Website.Models.ResponseModels
{
    public class ChartResponseModel
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<int> Data { get; set; } = new List<int>();
        public HttpStatusCode Status { get; set; }
    }
}
