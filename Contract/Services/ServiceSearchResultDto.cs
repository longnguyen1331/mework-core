namespace Contract.Services
{
    public class ServiceSearchResultDto
    {
        public int TotalItems { get; set; }
        public List<ServiceDto> Items { get; set; } = new List<ServiceDto>();
    }
}
