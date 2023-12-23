namespace Contract.Services
{
    public class ServiceFilterPagingDto : BaseFilterPagingDto
    {
        public bool? IsHighLight { get; set; }
        public int? MaxAge { get; set; }
        public int? MinimunAge { get; set; }
        public int? Gender { get; set; }
        public string? ICD10 { get; set; }
        public string? Tag { get; set; }
        public Guid? ServiceTypeId { get; set; }
        public string? ServiceTypeCode { get; set; }
        public string? ServiceSlug { get; set; }
        public Guid? IgnoreServiceId { get; set; }
        public Guid? DoctorId { get; set; }
        public bool? IsTopViews { get; set; }
    }
}
