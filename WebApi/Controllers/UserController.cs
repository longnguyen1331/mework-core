using Application.Identity.UserManager;
using Contract;
using Contract.Identity.UserManager;
using Contract.Uploads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/user/")]
    [Authorize]
    public class UserController : IUserManagerService
    {
        private UserManagerService _userManagerService;

        public UserController(UserManagerService userManagerService)
        {
            _userManagerService = userManagerService;
        }

        [HttpPost]
        [Route("create-firebase-token")]
        [AllowAnonymous]
        public async Task<ApiResponseBase<bool>> CreateTokenFireBase(TokenFribaseModel input)
        {
            return await _userManagerService.CreateTokenFireBase(input);
        }

        [HttpGet]   
        [Route("get-list-with-nav")]
        public async Task<List<UserWithNavigationPropertiesDto>> GetListWithNavigationAsync()
        {
            
           return  await _userManagerService.GetListWithNavigationAsync();
        }

       

        [HttpGet]   
        [Route("get-with-nav-properties/{id}")]
        public async Task<UserWithNavigationPropertiesDto> GetWithNavigationProperties(Guid id)
        {
            return await _userManagerService.GetWithNavigationProperties(id);
        }

        [HttpPost]
        [Route("create-user-with-roles")]
        public async Task<UserDto> CreateUserWithNavigationPropertiesAsync(CreateUserDto input)
        {
            return await _userManagerService.CreateUserWithNavigationPropertiesAsync(input);
        }
        
        [HttpPost]
        [Route("update-user-with-roles/{id}")]
        public async Task<UserDto> UpdateUserWithNavigationPropertiesAsync(UpdateUserDto input, Guid id)
        {
            return await _userManagerService.UpdateUserWithNavigationPropertiesAsync(input,id);
        }

    

        [HttpPost]
        [Route("create-users-from-csv-file")]
        [AllowAnonymous]
        public async Task<UserValidatorExcel> CreateUsersFromCSVFileAndDefineRoles(FileDto file)
        {
            return await _userManagerService.CreateUsersFromCSVFileAndDefineRoles(file);
        }


        [HttpPost]
        [Route("delete-with-nav")]
        public async  Task DeleteWithNavigationAsync(Guid id)
        {
             await _userManagerService.DeleteWithNavigationAsync(id);
        }

        [HttpGet]
        public async Task<List<UserDto>> GetListAsync()
        {
            return await _userManagerService.GetListAsync();
        }

        [HttpGet]
        [Route("get-basic-user-info")]
        public async Task<List<UserIdentityDto>> GetBasicUserInfosAsync()
        {
            return await _userManagerService.GetBasicUserInfosAsync();
        }

        [HttpPost]
        public async Task<UserDto> CreateAsync(CreateUserDto input)
        {
            return await _userManagerService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<UserDto> UpdateAsync(UpdateUserDto input, Guid id)
        {
            return await _userManagerService.UpdateAsync(input, id);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
             await _userManagerService.DeleteAsync(id);
        }
        
        
        [HttpPost]
        [Route("sign-in")]
        [AllowAnonymous]
        public async Task<TokenDto> SignInAsync(UserModel input)
        {
            return await _userManagerService.SignInAsync(input);
        }
        

        [HttpPost]
        [Route("sign-up")]
        [AllowAnonymous]
        public async Task<UserDto> SignUpAsync(CreateUserDto input)
        {
            return await _userManagerService.SignUpAsync(input);
        }

        [HttpPost]
        [Route("update-profile")]
        public async Task<UserDto> UpdateProfile(UpdateUserProfileRequestDto input)
        {
            return await _userManagerService.UpdateProfile(input);
        }

        [HttpPost]
        [Route("set-password")]
        public async Task<bool> SetNewPasswordAsync(NewUserPasswordDto input)
        {
            return await _userManagerService.SetNewPasswordAsync(input);
        }

        [HttpPost]
        [Route("refresh-token")]
        [AllowAnonymous]
        public async Task<TokenDto> RefreshTokenAsync(TokenModel token)
        {
            return await _userManagerService.RefreshTokenAsync(token);
        }

        [HttpGet]
        [Route("get-with-roles/{roleName}")]
        public async Task<List<UserDto>> GetListByRoles(string roleName)
        {
            UserFilterPagingModel filter = new UserFilterPagingModel();

            filter.FilterText = roleName;
            filter.Take = 0;
            return await _userManagerService.GetListByRoles(filter);
        }
        
        [HttpGet]
        [Route("get-with-roles/{createBy}/{roleName}")]
        public async Task<List<UserDto>> GetListByRoles(string createBy, string roleName)
        {
            UserFilterPagingModel filter = new UserFilterPagingModel();

            filter.FilterText = roleName;
            filter.CreatedBy = Guid.TryParse(createBy, out Guid userId) && userId != Guid.Empty ? userId : null;
            filter.Take = 0;

            return await _userManagerService.GetListByRoles(filter);
        }

		[ApiExplorerSettings(IgnoreApi = true)]
        public Task<List<UserDto>> GetListByRoles(UserFilterPagingModel? filter = null)
        {
            throw new NotImplementedException();
        }



        [HttpPost]
        [Route("get-with-filters")]
        public async Task<ApiResponseBase<List<UserDto>>> GetListByFilterAsync(UserFilterPagingModel filter)
        {
            return await _userManagerService.GetListByFilterAsync(filter);
        }

        [HttpPost]
        [Route("sign-out")]
        public async Task Logout(UserLogOutModel input)
        {
            await _userManagerService.Logout(input);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign-out-website")]
        public async Task LogoutWebsite(WebisteUserLogOutModel token)
        {
            await _userManagerService.LogoutWebsite(token);
        }




        [HttpPost]
        [Route("get-user-company-not-implement")]
        public Task<List<UserWithNavigationPropertiesDto>> GetUserCompanyListWithNavigationAsync(UserCompanyFilter filter)
        {
            throw new NotImplementedException();
        }
    }
}