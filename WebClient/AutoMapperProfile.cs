using AutoMapper;
using Contract;
using Contract.AppConfigs;
using Contract.Departments;
using Contract.DocumentFiles;
using Contract.DocumentTypes;
using Contract.FileFolders;
using Contract.Identity.RoleManager;
using Contract.Identity.UserManager;
using Contract.Positions;
using Contract.PostCategories;
using Contract.Posts;
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


           
            CreateMap<UserDto, CreateUserDto>().ReverseMap();



            CreateMap<CreateUpdatePositionDto, PositionDto>().ReverseMap();

            CreateMap<DepartmentDto, CreateUpdateDepartmentDto>().ReverseMap();

            CreateMap<FileFolderDto, CreateUpdateFileFolderDto>().ReverseMap();

        

            CreateMap<DocumentTypeDto, CreateUpdateDocumentTypeDto>().ReverseMap();
            CreateMap<DocumentFileDto, CreateUpdateDocumentFileDto>().ReverseMap();

            CreateMap<AppConfigDto, CreateUpdateAppConfigDto>().ReverseMap();


       
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