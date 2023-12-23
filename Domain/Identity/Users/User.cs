using Core.Enum;
using Domain.AppHistories;
using Domain.Positions;
using Domain.PostCategories;
using Domain.Posts;
using Domain.Services;
using Domain.ServiceTypes;
using Domain.UserDepartments;
using Domain.WebBanners;
using Domain.WebMenus;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Identity.Users
{
    public class User : IdentityUser<Guid>
    {
        public string UserCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; } = Gender.Unknown;
        public DateTime DOB { get; set; } = DateTime.Now;
        
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; }
        public string? RefreshToken { get; set;}
        
        public string? AvatarURL { get; set; }
        

        
        public Guid? PositionId { get; set; }


        //Patient
        [Column(TypeName = "varchar(50)")]
        public string? CitizenIDNumber { get; set; }
        public string? Address { get; set; }
        public Guid? DependsId { get; set; }
        public User? UserDepend { get; set; }
        public int? Relationship { get; set; }


        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }



        public List<PostCategory> CreatedUserPostCategories { get; set; }
        public List<PostCategory> ModifiedUserPostCategories { get; set; }


        public List<WebBanner> CreatedUserWebBanners { get; set; }
        public List<WebBanner> ModifiedUserWebBanners { get; set; }

        public List<WebMenu> CreatedUserWebMenus { get; set; }
        public List<WebMenu> ModifiedUserWebMenus { get; set; }

        public List<AppHistory> AppHistoryUsers { get; set; }

        public List<Post> CreatedUserPosts { get; set; }
        public List<Post> ModifiedUserPosts { get; set; }

        public Position Position { get; set; }
        public List<UserDepartment> UserDepartments { get; set;}



        public List<ServiceType> CreatedUserServiceTypes { get; set; }
        public List<ServiceType> ModifiedUserServiceTypes { get; set; }

        public List<Service> CreatedUserServices { get; set; }
        public List<Service> ModifiedUserServices { get; set; }
    }
}