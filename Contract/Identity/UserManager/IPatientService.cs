namespace Contract.Identity.UserManager
{
    public interface IPatientService
    {
        public Task<ApiResponseBase<List<UserDto>>> GetListByRoles(UserFilterPagingModel filter);
        public Task<ApiResponseBase<UserWithNavigationPropertiesDto>> GetWithNavigationProperties(Guid id);
        public Task<ApiResponseBase<UserDto>> CreateUserWithNavigationPropertiesAsync(CreateUserDto input);
        public Task<ApiResponseBase<UserDto>> UpdateUserWithNavigationPropertiesAsync(UpdateUserDto input, Guid id);
        public Task<ApiResponseBase<UserDto>> DeleteAsync(Guid id);
        public Task<UserDto> CreateAsync(CreateUserDto input);
        public Task<UserDto> UpdateAsync(UpdateUserDto input, Guid id);
    }
}
