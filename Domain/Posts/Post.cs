using Domain.AppConfigs;
using Domain.PostCategories;
using Domain.StaticFiles;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Posts
{
    public class Post : BaseEntity 
    {
        public Post() 
        { 
            ODX = 0;
            IsDeleted = false;
            IsVisibled = false;
            CreatedDate = DateTime.Now;
        }

        [MaxLength(512)]
        public string Title { get; set; }
        public Guid PostCategoryId { get; set; }

        public string? SeoTitle { get; set; }
        public string? Tags { get; set; }
        public string? Slug { get; set; }
        public string? SeoKeyword { get; set; }
        [Column(TypeName = "ntext")]
        public string? SeoDescription { get; set; }


        [MaxLength(512)]
        public string? SortDescription { get; set; }

        [Column(TypeName = "ntext")]
        public string? Description { get; set; }

        public int Views { get; set; } = 0;
        public int ODX { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsHighLight { get; set; }
        public bool IsVisibled { get; set; }
        public Guid? PictureId { get; set; }
        public StaticFile? Picture { get; set; }
        public PostCategory PostCategory { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}