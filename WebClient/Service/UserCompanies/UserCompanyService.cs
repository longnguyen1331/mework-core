using Contract.Identity.UserManager;
using WebClient.RequestHttp;

namespace WebClient.Service.UserCompanies
{
    public class UserCompanyService 
    {
        public async Task<List<UserWithNavigationPropertiesDto>> GetUserCompanyListWithNavigationAsync(UserCompanyFilter filter)
        {
            return await RequestClient.PostAPIAsync<List<UserWithNavigationPropertiesDto>>("user-companies", filter);

        }
    }
}