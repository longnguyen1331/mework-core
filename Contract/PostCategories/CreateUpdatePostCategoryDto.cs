using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Contract.PostCategories
{
	public class CreateUpdatePostCategoryDto
    {
		public Guid Id { get; set; }
        [Required(ErrorMessage = "Please enter category name.")]
        public string Name { get; set; }
		public Guid? ParentCategoryId { get; set; }

        [Required(ErrorMessage = "Please enter title.")]
		public string SeoTitle { get; set; }
        [RegularExpression(@"^[a-z\d](?:[a-z\d_-]*[a-z\d])?$", ErrorMessage = "Please enter correct slug.")]
        public string Slug { get; set; }
        [Required(ErrorMessage = "Please enter keyword.")]
        public string SeoKeyword { get; set; }
        [Required(ErrorMessage = "Please enter description.")]
        public string SeoDescription { get; set; }

        public int ODX { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsHighLight { get; set; }
		public bool IsVisibled { get; set; }
		public Guid? ImageId { get; set; }
		public Guid? CreatedBy { get; set; }
		public Guid? ModifiedBy { get; set; }
	}
}
