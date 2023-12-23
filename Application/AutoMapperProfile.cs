using AutoMapper;
using Contract.AppConfigs;
using Contract.AppHistories;
using Contract.Departments;
using Contract.Files;
using Contract.Identity.RoleManager;
using Contract.Identity.UserManager;
using Contract.Positions;
using Contract.PostCategories;
using Contract.Posts;
using Contract.Services;
using Contract.ServiceTypes;
using Contract.WebBanners;
using Contract.WebMenus;
using Core.Extension;
using Core.Helper;
using Domain.AppConfigs;
using Domain.AppHistories;
using Domain.Departments;
using Domain.Identity.Roles;
using Domain.Identity.Users;
using Domain.Positions;
using Domain.PostCategories;
using Domain.Posts;
using Domain.Services;
using Domain.ServiceTypes;
using Domain.StaticFiles;
using Domain.WebBanners;
using Domain.WebMenus;

namespace Application
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppHistoryDto, AppHistory>();
            CreateMap<AppHistory, AppHistoryDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => (src.User.FirstName + " " + src.User.LastName).Trim()));
            CreateMap<CreateUpdateAppHistoryDto, AppHistory>().ReverseMap();


            CreateMap<UserWithNavigationProperties, UserWithNavigationPropertiesDto>().ReverseMap();
            CreateMap<CreateUserDto, User>().ReverseMap();
            CreateMap<UpdateUserDto, User>().ReverseMap();
            CreateMap<UpdateUserProfileRequestDto, User>().ReverseMap();
            CreateMap<CreateUpdateUseDto, CreateUserDto>();
            CreateMap<CreateUpdateUseDto, UpdateUserDto>();
            CreateMap<User, UserDto>().ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
            CreateMap<User, UserIdentityDto>().ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
            CreateMap<CreateUpdateUseDto, User>().ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<RoleDto, CreateUpdateRoleDto>().ReverseMap();
            CreateMap<Role, CreateUpdateRoleDto>().ReverseMap();

            CreateMap<Position, PositionDto>().ReverseMap();
            CreateMap<CreateUpdatePositionDto, Position>().ReverseMap();


            CreateMap<Department, DepartmentDto>().ReverseMap();
            CreateMap<CreateUpdateDepartmentDto, Department>().ReverseMap();


            CreateMap<AppConfig, AppConfigDto>();
            CreateMap<CreateUpdateAppConfigDto, AppConfig>();

            CreateMap<AppConfig, AppConfigDto>()
                .ForMember(dest => dest.IconURL,
                    opt => opt.MapFrom(src => src.IconFile != null  ? src.IconFile.URL : ""))
                .ForMember(dest => dest.LogoURL,
                    opt => opt.MapFrom(src => src.LogoFile != null ?  src.LogoFile.URL : ""));

            CreateMap<StaticFile, StaticFileDto>();
       
            CreateMap<UserIdentity, UserIdentityDto>().ReverseMap();

            CreateMap<StaticFile, StaticFileDto>();
            CreateMap<FileModel, StaticFile>();
            CreateMap<CreateUpdateStaticFileDto, StaticFile>();

            CreateMap<WebBannerDto, WebBanner>().ReverseMap();
            CreateMap<CreateUpdateWebBannerDto, WebBanner>().ReverseMap();

            CreateMap<WebMenuDto, WebMenu>().ReverseMap();


            CreateMap<WebMenuDto, WebMenu>();
            CreateMap<WebMenu, WebMenuDto>()
                .ForMember(dest => dest.ImageUrl,
                    opt => opt.MapFrom(src => src.Image.URL));

            CreateMap<CreateUpdateWebMenuDto, WebMenu>().ReverseMap();

            CreateMap<PostCategoryDto, PostCategory>();
            CreateMap<PostCategory, PostCategoryDto>()
                .ForMember(dest => dest.ImageUrl,
                    opt => opt.MapFrom(src => src.Image.URL));
            CreateMap<CreateUpdatePostCategoryDto, PostCategory>().ReverseMap();


            CreateMap<PostDto, Post>();
            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.PostCategoryName,
                    opt => opt.MapFrom(src => (src.PostCategory.Name).Trim()))
                .ForMember(dest => dest.PosterFullName,
                    opt => opt.MapFrom(src => src.CreatedUser != null ? (src.CreatedUser.FirstName + " " + src.CreatedUser.LastName).Trim() : string.Empty))
                .ForMember(dest => dest.PictureUrl,
                    opt => opt.MapFrom(src => src.Picture != null ? src.Picture.URL : string.Empty));
            CreateMap<CreateUpdatePostDto, Post>().ReverseMap();
            CreateMap<ServiceTypeDto, ServiceType>();
            CreateMap<ServiceType, ServiceTypeDto>()
                .ForMember(dest => dest.ImageUrl,
                    opt => opt.MapFrom(src => src.ImageFile.URL))
                .ForMember(dest => dest.Slug,
                    opt => opt.MapFrom(src => src.Name.GenerateSlug()));


            CreateMap<ServiceDto, Service>();
            CreateMap<Service, ServiceDto>()
                .ForMember(dest => dest.ServiceTypeCode,
                    opt => opt.MapFrom(src => src.ServiceType.Code))
                .ForMember(dest => dest.ServiceTypeName,
                    opt => opt.MapFrom(src => src.ServiceType.Name))
                .ForMember(dest => dest.ImageUrl,
                    opt => opt.MapFrom(src => src.ImageFile.URL));
            CreateMap<CreateUpdateServiceDto, Service>().ReverseMap();


            CreateMap<CreateUpdateServiceTypeDto, ServiceType>().ReverseMap();

        }
    }
}