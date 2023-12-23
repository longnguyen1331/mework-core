namespace Contract.Posts
{
    public class PostDto
    {
        public Guid Id { set; get; }
        public string Title { get; set; }
        public Guid PostCategoryId { get; set; }
        public string? Tags { get; set; }
        public string? SeoTitle { get; set; }
        public string? Slug { get; set; }
        public string? SeoKeyword { get; set; }
        public string? SeoDescription { get; set; }
        public string? SortDescription { get; set; }
        public string? Description { get; set; }
        public int Views { get; set; } = 0;
        public int ODX { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsHighLight { get; set; }
        public bool IsVisibled { get; set; }
        public Guid? PictureId { get; set; }
        public string? PictureUrl { get; set; }
        public string PostCategoryName { get; set; }
        public string? PosterFullName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PostedDate
        {
            get
            {
                return CreatedDate.ToString("dd/MM/yyyy");
            }
        }
    }
}
