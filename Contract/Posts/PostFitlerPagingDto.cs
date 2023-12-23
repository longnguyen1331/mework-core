namespace Contract.Posts
{
    public class PostFitlerPagingDto : BaseFilterPagingDto
    {
        public string? Tags { get; set; }
        public Guid? PostCategoryId { get; set; }
        public string? PostCategoryIds { get; set; }
        public bool? IsCategoryHighlight { get; set; }
        public bool? IsHighlight { get; set; }
        public bool? IsTopViews { get; set; }
        public string? PostCategorySlug { get; set; }
        public string? PostSlug { get; set; }
        public Guid? IgnorePostId { get; set; }
    }
}
