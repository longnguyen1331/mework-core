using System;

namespace Contract.WebMenus
{
    public class CreateUpdateWebMenuDto
    {
        public CreateUpdateWebMenuDto()
        {
            IsVisibled = true;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Note { get; set; }
        public string? UrlRef { get; set; }
        public int MenuType { get; set; }
        public string? MenuSuffix { get; set; }
        public Guid? ParentMenuId { get; set; }
        public string SeoTitle { get; set; }
        public string Slug { get; set; }
        public string SeoKeyword { get; set; }
        public string SeoDescription { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsVisibled { get; set; }
        public string? ImageUrl { get; set; }
        public int ODX { get; set; }
        public Guid? ImageId { get; set; }
    }
}