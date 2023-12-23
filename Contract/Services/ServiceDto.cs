using Contract.Files;

namespace Contract.Services
{
    public class ServiceDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public int ODX { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsHighLight { get; set; }
        public bool IsVisibled { get; set; }
        public Guid? ImageId { get; set; }

        public StaticFileDto? ImageFile { get; set; }
        public Guid? ServiceTypeId { get; set; }
        public string ServiceTypeCode { get; set; }
        public string ServiceTypeName { get; set; }
        public decimal Charges { get; set; }
        public decimal OldCharges { get; set; }
        public string Tags { get; set; }
        public string SeoTitle { get; set; }
        public string Slug { get; set; }
        public string SeoKeyword { get; set; }
        public string SeoDescription { get; set; }
        public string Note { get; set; }
        public int Views { get; set; }
    }
}