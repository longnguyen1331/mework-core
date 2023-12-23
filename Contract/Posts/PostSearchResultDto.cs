namespace Contract.Posts
{
    public class PostSearchResultDto
    {
        public int TotalItems { get; set; }
        public List<PostDto> Items { get; set; } = new List<PostDto>();
    }
}
