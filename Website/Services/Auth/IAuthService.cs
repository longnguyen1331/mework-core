using Contract;
using Contract.Identity.UserManager;
using Contract.Uploads;
using Website.Models;

namespace Website.Services.Auth
{
    public interface IAuthService
    {
        Task<IDictionary<string, object>?> AuthAsync(UserModel input);
        Task SignOutAsync(WebisteUserLogOutModel token);
        string? GetDeviceId();
        string? GetUserToken();
        string? GetUserCode();
        string? GetUserId();
        Task<ApiResponseBase<UserDto>?> SignUpAsync(CreateUserDto input);
        Task<ApiResponseBase<UserDto>?> UpdateAsync(UpdateUserModel input, Guid id);
        Task<FileDto?> UploadImage(IFormFile file);
    }
}
