using System.Threading.Tasks;
using Blazored.LocalStorage;
using Core.Enum;

namespace WebClient.Pages.Admin
{
    public partial class Logout
    {


        protected override async Task OnInitializedAsync()
        {
            await InvokeAsync( async () =>
            {
                _navigationManager.NavigateTo("/login");
                _userManagerService.Logout();
            }, ActionType.SignOut, true);
  
        }

    }
}