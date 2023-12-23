using Contract.Identity.UserManager;
using Domain.Identity.Users;

namespace SqlServ4r.Repository.Users
{
    public interface IUserRepository
    {
        (bool ExistEmail, bool ExistPhoneNumber,bool ExistUserCode) CheckDuplicateInformation(string email,string phone,string userCode);
        (bool ExistEmail, bool ExistPhoneNumber,bool ExistUserCode) CheckDuplicateInformation(string email,string phone,string userCode,Guid id);

        Task<List<UserWithNavigationProperties>> GetListWithNavigationProperties();
        Task<UserWithNavigationProperties> GetWithNavigationProperties(Guid id);
        Task<List<User>> GetListByRoles(UserFilterPagingModel? filter = null);
    }
}