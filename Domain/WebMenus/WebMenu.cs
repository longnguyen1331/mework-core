using Domain.AppConfigs;
using Domain.StaticFiles;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.WebMenus
{
    public class WebMenu : BaseEntity 
    {
        public WebMenu() 
        { 
            ODX = 0;
            IsDeleted = false;
            IsVisibled = false;
        }

        [MaxLength(128)]
        public string Name { get; set; }
        public string? UrlRef { get; set; }
        [MaxLength(128)]
        public string MenuType { get; set; }
        [MaxLength(128)]
        public string? MenuSuffix { get; set; }
        public Guid? ParentMenuId { get; set; }
        
        public string? Note { get; set; }
        public string? SeoTitle { get; set; }
        public string? Slug { get; set; }
        public string? SeoKeyword { get; set; }
        [Column(TypeName = "ntext")]
        public string? SeoDescription { get; set; }
        public int ODX { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsVisibled { get; set; }
        public Guid? ImageId { get; set; }
        public StaticFile? Image { get; set; }
    }
}