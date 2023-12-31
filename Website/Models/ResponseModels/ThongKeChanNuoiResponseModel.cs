using System.Text.Json.Serialization;

namespace Website.Models.ResponseModels
{
    public class ThongKeChanNuoiResponseModel
    {
        public IEnumerable<ThongKeChanNuoiChiTietResponseModel>? Datas { get; set; }
    }
    public class ThongKeChanNuoiChiTietResponseModel
    {
        [JsonPropertyName("id")]
        public int Id { set; get; } = 0;
        [JsonPropertyName("vatNuoi")]
        public string VatNuoi { set; get; } = string.Empty;
        [JsonPropertyName("soLuongNuoi")]
        public int SoLuongNuoi { set; get; } = 0;
        [JsonPropertyName("sanLuongTrung")]
        public int SanLuongTrung { set; get; } = 0;
        [JsonPropertyName("idXaPhuong")]
        public int IdXaPhuong { set; get; } = 0;
        [JsonPropertyName("idQuanHuyen")]
        public int IdQuanHuyen { set; get; } = 0;
        [JsonPropertyName("idTinhThanh")]
        public int IdTinhThanh { set; get; } = 0;
        [JsonPropertyName("xaPhuong")]
        public string xaPhuong { set; get; } = string.Empty;
        [JsonPropertyName("quanHuyen")]
        public string QuanHuyen { set; get; } = string.Empty;
        [JsonPropertyName("tinhThanh")]
        public string TinhThanh { set; get; } = string.Empty;
    }
}
