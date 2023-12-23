using Core.Enum;
using Domain.AppConfigs;
using Domain.ServiceTypes;
using Domain.StaticFiles;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Services
{
    public class Service : BaseEntity
    {
        public Service() 
        { 
            ODX = 0;
            IsDeleted = false;
            IsHighLight = false;
            IsVisibled = false;
        }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public Guid? ImageId { get; set; }
        public StaticFile? ImageFile { get; set; }
        public Guid? ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }
        public decimal Charges { get; set; }
        public decimal OldCharges { get; set; }
        [Column(TypeName = "ntext")]
        public string? Description { get; set; }
        [Column(TypeName = "ntext")]
        public string? Tags { get; set; }
        [Column(TypeName = "ntext")]
        public string? SeoTitle { get; set; }
        public string? Slug { get; set; }
        public string? SeoKeyword { get; set; }
        [Column(TypeName = "ntext")]
        public string? SeoDescription { get; set; }
        [Column(TypeName = "ntext")]
        public string? Note { get; set; }
        public int Views { get; set; } = 0;
        public bool IsDeleted { get; set; }
        public bool IsHighLight { get; set; }
        public bool IsVisibled { get; set; }
        public int ODX { get; set; }

	}
}