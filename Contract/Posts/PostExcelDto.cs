namespace Contract.Posts
{
    public class PostExcelDto
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? PostCategoryId { get; set; }
        public string? Tags { get; set; }
        public string? SeoTitle { get; set; }
        public string? Slug { get; set; }
        public string? SeoKeyword { get; set; }
        public string? SeoDescription { get; set; }
        public string? SortDescription { get; set; }
        public string? Description { get; set; }
        public string? ODX { get; set; }
        public string? IsHighLight { get; set; }
        public string? IsVisibled { get; set; }
        public string? PictureId { get; set; }
        public int Row { set; get; }
        public string? CreateDate { get; set; }
        public string? UserId { get; set; }
    }
}
