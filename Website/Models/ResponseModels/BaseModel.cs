using System.Text.Json.Serialization;

namespace Website.Models.ResponseModels
{
    public class BaseModel
    {
        [JsonPropertyName("id")]
        public int Id  { get; set; }
        [JsonPropertyName("ten")]
        public string? Ten  { get; set; }



        [JsonPropertyName("selected")]
        public bool Selected { get; set; } =false;
        [JsonPropertyName("disabled")]
        public bool Disabled { get; set; } = false;
    }
}
