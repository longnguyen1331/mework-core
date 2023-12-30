using AutoMapper;
using Contract;
using Contract.AppConfigs;
using Contract.Backups;
using Contract.Departments;
using Contract.Identity.RoleManager;
using Contract.Identity.UserManager;
using Contract.Positions;
using Contract.PostCategories;
using Contract.Posts;
using Contract.Services;
using Contract.ServiceTypes;
using Contract.WebBanners;
using Contract.WebMenus;

namespace WebClient
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            
            CreateMap<RoleDto, CreateUpdateRoleDto>().ReverseMap();

            CreateMap<UserDto, CreateUserDto>().ReverseMap();
            CreateMap<UserDto, UpdateUserDto>().ReverseMap();
            CreateMap<UpdateUserDto, CreateUserDto>().ReverseMap();
            CreateMap<UserWithNavigationPropertiesDto, CreateUserDto>().ReverseMap();


            CreateMap<CreateUpdateBackupDto, BackupDto>().ReverseMap();

            CreateMap<UserDto, CreateUserDto>().ReverseMap();
            CreateMap<CreateUpdatePositionDto, PositionDto>().ReverseMap();

            CreateMap<DepartmentDto, CreateUpdateDepartmentDto>().ReverseMap();

            CreateMap<AppConfigDto, CreateUpdateAppConfigDto>().ReverseMap();



            CreateMap<ServiceTypeDto, Domain.ServiceTypes.ServiceType>().ReverseMap();
            CreateMap<ServiceTypeDto, CreateUpdateServiceTypeDto>().ReverseMap();

            CreateMap<ServiceDto, Domain.Services.Service>().ReverseMap();
            CreateMap<ServiceDto, CreateUpdateServiceDto>().ReverseMap();
            CreateMap<UserDto, UserIdentityDto>();
            CreateMap<UserDto, UpdateUserProfileRequestDto>();

            CreateMap<WebBannerDto, Domain.WebBanners.WebBanner>().ReverseMap();
            CreateMap<WebBannerDto, CreateUpdateWebBannerDto>().ReverseMap();
            CreateMap<WebMenuDto, Domain.WebMenus.WebMenu>().ReverseMap();
            CreateMap<WebMenuDto, CreateUpdateWebMenuDto>().ReverseMap();

            CreateMap<PostCategoryDto, Domain.PostCategories.PostCategory>().ReverseMap();
            CreateMap<CreateUpdatePostCategoryDto, Domain.PostCategories.PostCategory>().ReverseMap();
            CreateMap<PostCategoryDto, CreateUpdatePostCategoryDto>().ReverseMap();
            CreateMap<PostCategoryDto, BaseDto>();

            CreateMap<PostDto, Domain.Posts.Post>().ReverseMap();
            CreateMap<CreateUpdatePostDto, Domain.Posts.Post>().ReverseMap();
            CreateMap<PostDto, CreateUpdatePostDto>().ReverseMap();

        }
    }
}