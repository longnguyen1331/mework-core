using System.Text.Json.Serialization;

namespace Website.Models.ResponseModels
{
    public class ThongKeResponseModel
    {
        [JsonPropertyName("tongSoCoSoChanNuoi")]
        public int TongSoCoSoChanNuoi { set; get; } = 0;
        [JsonPropertyName("coSoChanNuoiLon")]
        public int CoSoChanNuoiLon { set; get; } = 0;
        [JsonPropertyName("coSoChanNuoiTrau")]
        public int CoSoChanNuoiTrau { set; get; } = 0;
        [JsonPropertyName("coSoChanNuoiBo")]
        public int CoSoChanNuoiBo { set; get; } = 0;
        [JsonPropertyName("coSoChanNuoiGa")]
        public int CoSoChanNuoiGa { set; get; } = 0;
        [JsonPropertyName("coSoChanNuoiVit")]
        public int CoSoChanNuoiVit { set; get; } = 0;
        [JsonPropertyName("tongSoCoSoSxThucAn")]
        public int TongSoCoSoSxThucAn { set; get; } = 0;
        [JsonPropertyName("nongHo")]
        public int NongHo { set; get; } = 0;
        [JsonPropertyName("trangTraiNho")]
        public int TrangTraiNho { set; get; } = 0;
        [JsonPropertyName("trangTraiVua")]
        public int TrangTraiVua { set; get; } = 0;
        [JsonPropertyName("trangTraiLon")]
        public int TrangTraiLon { set; get; } = 0;
        [JsonPropertyName("sanLuongThitGiaSuc")]
        public int SanLuongThitGiaSuc { set; get; } = 0;
        [JsonPropertyName("sanLuongThitGiaCam")]
        public int SanLuongThitGiaCam { set; get; } = 0;
        [JsonPropertyName("sanLuongTrung")]
        public int SanLuongTrung { set; get; } = 0;
        [JsonPropertyName("sanLuongSua")]
        public int SanLuongSua { set; get; } = 0;
        [JsonPropertyName("tongCoSoSxTACN")]
        public int TongCoSoSxTACN { set; get; } = 0;
        [JsonPropertyName("tongCoSoKdTACN")]
        public int TongCoSoKdTACN { set; get; } = 0;
        [JsonPropertyName("tongDanLon")]
        public int TongDanLon { set; get; } = 0;
        [JsonPropertyName("tongDanBo")]
        public int TongDanBo { set; get; } = 0;
        [JsonPropertyName("tongDanTrau")]
        public int TongDanTrau { set; get; } = 0;
        [JsonPropertyName("tongDanGiaCam")]
        public int TongDanGiaCam { set; get; } = 0;
    }
}
