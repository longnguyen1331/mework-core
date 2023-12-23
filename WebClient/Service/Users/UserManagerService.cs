using Blazored.LocalStorage;
using Contract;
using Contract.Identity.UserManager;
using Contract.Uploads;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using WebClient.Identity;
using WebClient.RequestHttp;

namespace WebClient.Service.Users
{
    public class UserManagerService : IUserManagerService
    {
        private ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider  _authenticationStateProvider;
        private readonly IJSRuntime _jsRuntime;

        public UserManagerService(
            ILocalStorageService localStorage,
            IJSRuntime jsRuntime,
            AuthenticationStateProvider  authenticationStateProvider)
        {
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
            _jsRuntime = jsRuntime;
        }


        public async Task<List<UserWithNavigationPropertiesDto>> GetListWithNavigationAsync()
        {   
             return await RequestClient.GetAPIAsync<List<UserWithNavigationPropertiesDto>>("user/get-list-with-nav");
        }
        public async Task<List<UserWithNavigationPropertiesDto>> GetUserCompanyListWithNavigationAsync(UserCompanyFilter filter)
        {
            return await RequestClient.PostAPIAsync<List<UserWithNavigationPropertiesDto>>("user/get-user-company-list-with-nav", filter);
        }

        public async Task<List<UserBasicInfoDto>> GetBasicInfoUsersWithNavigationAsync(PhoneBookFilter filter)
        {
            return await RequestClient.PostAPIAsync<List<UserBasicInfoDto>>("user/get-basic-info-users-with-navigation-properties",filter);
        }

        public async Task<UserWithNavigationPropertiesDto> GetWithNavigationProperties(Guid id)
        {
            return await RequestClient.GetAPIAsync<UserWithNavigationPropertiesDto>($"user/get-with-nav-properties/{id}");
        }

        public async Task<UserDto> CreateUserWithNavigationPropertiesAsync(CreateUserDto input)
        {
            return await RequestClient.PostAPIAsync<UserDto>("user/create-user-with-roles", input);
        }

        public async Task<UserDto> UpdateUserWithNavigationPropertiesAsync(UpdateUserDto input, Guid id)
        {
            return await RequestClient.PostAPIAsync<UserDto>($"user/update-user-with-roles/{id}", input);
        }

    
        public async Task<List<UserIdentityDto>> GetBasicUserInfosAsync()
        {
            return await RequestClient.GetAPIAsync<List<UserIdentityDto>>("user/get-basic-user-info");
        }


        public Task<UserDto> UpdateUserWithRolesByPhoneNumberAsync(UpdateUserDto input, string phoneNumber)
        {
            throw new NotImplementedException();
        }
        
        public async Task<UserValidatorExcel> CreateUsersFromCSVFileAndDefineRoles(FileDto file)
        {
            return await RequestClient.PostAPIAsync<UserValidatorExcel>($"user/create-users-from-csv-file", file);
        }

        public async Task DeleteWithNavigationAsync(Guid id)
        {
             await RequestClient.PostAPIAsync<Task>($"user/delete-with-nav/{id}",null);
        }

        public async Task<List<UserDto>> GetListAsync()
        {
            return await RequestClient.GetAPIAsync<List<UserDto>>("user");
        }

        public async Task<List<UserIdentityDto>> GetBasicUserInfoAsync()
        {
            return await RequestClient.GetAPIAsync<List<UserIdentityDto>>("user/get-basic-user-info");
        }

        public async Task<UserDto> CreateAsync(CreateUserDto input)
        {
            return await RequestClient.PostAPIAsync<UserDto>("user",input);
        }

        public async Task<UserDto> UpdateAsync(UpdateUserDto input, Guid id)
        {
            return await RequestClient.PutAPIAsync<UserDto>($"user/{id}",input);
        }

        public async Task DeleteAsync(Guid id)
        {
             await RequestClient.DeleteAPIAsync<Task>($"user/{id}");
        }

        public async Task<TokenDto> SignInAsync(UserModel input)
        {
            var response = await RequestClient.PostAPIAsync<TokenDto>("user/sign-in", input);
            RequestClient.AttachToken(response.AccessToken);
                 RequestClient.InjectServices(_localStorage);
                await _localStorage.SetItemAsync("my-access-token", response.AccessToken);
                await _localStorage.SetItemAsync("my-refresh-token", response.RefreshToken);
                ((ApiAuthenticationStateProvider) _authenticationStateProvider).MarkUserAsAuthenticated(input.UserName);
       
            return response;
        }
        

        public async Task<UserDto> SignUpAsync(CreateUserDto input)
        {
            return await RequestClient.PostAPIAsync<UserDto>("user/sign-up", input);
        }

        public async Task<UserDto> UpdateProfile(UpdateUserProfileRequestDto input)
        {
            return await RequestClient.PostAPIAsync<UserDto>("user/update-profile", input);
        }

        public async Task<bool> SetNewPasswordAsync(NewUserPasswordDto input)
        {
            return await RequestClient.PostAPIAsync<bool>("user/set-password", input);
        }


        public Task<UserProfileModel> UpdateUserProfileAsync(UserProfileModel userProfileModel)
        {
            throw new NotImplementedException();
        }


        

        public Task<TokenDto> RefreshTokenAsync(TokenModel token)
        {
            throw new NotImplementedException();
        }

        public async void Logout()
        {
           await ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        }

        public async void LogOutAndRemoveUserToken(WebisteUserLogOutModel input)
        {
            var response = await RequestClient.PostAPIAsync<WebisteUserLogOutModel>("user/sign-out-website", input);
            await ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        }

        public async Task<List<UserDto>> GetListByRoles(string roleName)
        {
            return await RequestClient.GetAPIAsync<List<UserDto>>($"user/get-with-roles/{roleName}");
        }
        
        public async Task<List<UserDto>> GetListByRoles(Guid createBy, string roleName)
        {
            return await RequestClient.GetAPIAsync<List<UserDto>>($"user/get-with-roles/{createBy}/{roleName}");
        }

        public Task<List<UserDto>> GetListByRoles(UserFilterPagingModel? filter = null)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponseBase<List<UserDto>>> GetListByFilterAsync(UserFilterPagingModel filter)
        {
            return await RequestClient.PostAPIAsync<ApiResponseBase<List<UserDto>>>($"user/get-with-filters", filter);
        }

        public async Task<ApiResponseBase<bool>> CreateTokenFireBase(TokenFribaseModel token)
        {
            return await RequestClient.PostAPIAsync<ApiResponseBase<bool>>($"user/create-firebase-token", token);
        }

        public Task LogoutWebsite(WebisteUserLogOutModel token)
        {
            throw new NotImplementedException();
        }

        
    }
}