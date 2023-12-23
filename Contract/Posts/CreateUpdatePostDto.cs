using Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Contract.Posts
{
    public class CreateUpdatePostDto
    {
        public Guid Id { set; get; }
        [Required(ErrorMessage = "Please enter Title.")]
        public string Title { get; set; }
        [GuidNotEmpty]
        public Guid PostCategoryId { get; set; }
        public string? Tags { get; set; }
        public string? SeoTitle { get; set; }
        [RegularExpression(@"^[a-z\d](?:[a-z\d_-]*[a-z\d])?$", ErrorMessage = "Please enter correct slug.")]
        public string? Slug { get; set; }
        public string? SeoKeyword { get; set; }
        public string? SeoDescription { get; set; }
        public string? SortDescription { get; set; }
        public string? Description { get; set; }
        public int Views { get; set; } = 0;
        public int ODX { get; set; } = 0;
        public bool IsDeleted { get; set; } = false;
        public bool IsHighLight { get; set; } = false;
        public bool IsVisibled { get; set; } = true;
        public Guid? PictureId { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
