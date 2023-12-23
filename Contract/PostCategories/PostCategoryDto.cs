namespace Contract.PostCategories
{
	public class PostCategoryDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public Guid? ParentCategoryId { get; set; }

		public string SeoTitle { get; set; } = string.Empty;
		public string Slug { get; set; } = string.Empty;
		public string SeoKeyword { get; set; } = string.Empty;
		public string SeoDescription { get; set; } = string.Empty;

		public int ODX { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsHighLight { get; set; }
		public bool IsVisibled { get; set; }
		public Guid? ImageId { get; set; }
		public string? ImageUrl { get; set; }
		public DateTime CreatedDate { get; set; }

    }
}
