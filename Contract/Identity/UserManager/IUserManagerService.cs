using Contract.Uploads;

namespace Contract.Identity.UserManager
{
    public interface IUserManagerService
    {
        public Task<List<UserWithNavigationPropertiesDto>> GetListWithNavigationAsync();
        public Task<List<UserWithNavigationPropertiesDto>> GetUserCompanyListWithNavigationAsync(UserCompanyFilter filter);


        public Task<UserWithNavigationPropertiesDto> GetWithNavigationProperties(Guid id);
        
        public Task<UserDto> CreateUserWithNavigationPropertiesAsync(CreateUserDto input);
        public Task<UserDto> UpdateUserWithNavigationPropertiesAsync(UpdateUserDto input,Guid id);

        public Task<UserValidatorExcel> CreateUsersFromCSVFileAndDefineRoles(FileDto file);
        
        public Task DeleteWithNavigationAsync(Guid id);
        public Task<List<UserDto>> GetListAsync();
        public Task<List<UserDto>> GetListByRoles(UserFilterPagingModel? filter = null);
        public Task<List<UserIdentityDto>> GetBasicUserInfosAsync();

        public Task<UserDto> CreateAsync(CreateUserDto input);
        public Task<UserDto> UpdateAsync(UpdateUserDto input,Guid id);
        public Task DeleteAsync(Guid id);
        public Task<TokenDto> SignInAsync(UserModel input);
        public Task<UserDto> SignUpAsync(CreateUserDto input);
        public Task<UserDto> UpdateProfile(UpdateUserProfileRequestDto input);
        public Task<bool> SetNewPasswordAsync(NewUserPasswordDto input);
        public Task<TokenDto> RefreshTokenAsync(TokenModel token);
        public Task<ApiResponseBase<List<UserDto>>> GetListByFilterAsync(UserFilterPagingModel? filter = null);
        public Task<ApiResponseBase<bool>> CreateTokenFireBase(TokenFribaseModel token);
        public Task LogoutWebsite(WebisteUserLogOutModel token);
    }
}