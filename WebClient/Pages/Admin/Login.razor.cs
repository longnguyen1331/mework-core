using System;
using System.Threading.Tasks;
using Contract.Identity.UserManager;
using Core.Enum;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;

namespace WebClient.Pages.Admin
{
    public partial class Login
    {

        public bool IsLoading = false;
        
        [Parameter]
        [SupplyParameterFromQuery(Name = "toURl")]
        public string ToURl { get; set; }
        public Login()
        {
            
        }

        protected async override Task OnInitializedAsync()
        {

            
            if (await IsAuthenticatedAsync())
            
            {
            
                _navigationManager.NavigateTo("/");
            }

        }
        
        async Task  OnLogin(LoginArgs args, string name)
        {

            var userModel = new UserModel() {UserName = args.Username, Password = args.Password};
            IsLoading = true;
            await InvokeAsync(async () =>
            {
                await  _userManagerService.SignInAsync(userModel);
                _navigationManager.NavigateTo(ToURl,true);
            }, ActionType.SignIn, true);
            IsLoading = false;

        }
        
    }
}