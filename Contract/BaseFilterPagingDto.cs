namespace Contract
{
    public class BaseFilterPagingDto
    {
        public string? FilterText { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;
        public string? Sort { set;get; }
    }

    public class BaseFilterByDateTimeDto : BaseFilterPagingDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
