using Domain.AppConfigs;
using Domain.Posts;
using Domain.StaticFiles;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.PostCategories
{
    public class PostCategory : BaseEntity 
    {
        public PostCategory() 
        { 
            ODX = 0;
            IsDeleted = false;
            IsVisibled = false;
        }

        [MaxLength(128)]
        public string Name { get; set; }
        public Guid? ParentCategoryId { get; set; }

        public string SeoTitle { get; set; }
        public string Slug { get; set; }
        public string SeoKeyword { get; set; }
        [Column(TypeName = "ntext")]
        public string SeoDescription { get; set; }

        public int ODX { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsHighLight { get; set; }
        public bool IsVisibled { get; set; }
        public Guid? ImageId { get; set; }
        public StaticFile Image { get; set; }

        public List<Post> Posts { get; set; }

    }
}