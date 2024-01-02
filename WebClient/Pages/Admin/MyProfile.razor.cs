using Contract.Identity.UserManager;
using Core.Enum;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using System;
using WebClient.Exceptions;
using WebClient.Setting;

namespace WebClient.Pages.Admin
{
    public partial class MyProfile
    {

        public TabPosition tabPosition = TabPosition.Left;
        public UserWithNavigationPropertiesDto UserDto { get; set; } = new UserWithNavigationPropertiesDto();
        private UpdateUserProfileRequestDto NewProfile = new UpdateUserProfileRequestDto();
        public NewUserPasswordDto NewPassword { get; set; } = new NewUserPasswordDto();
        private Radzen.FileInfo AvatarFile { get; set; }
        public string fileName, fileBase64;
        public long? fileSize;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                   await  GetProfile();
                },ActionType.LoadData);
                StateHasChanged();
         
            }
        }

        public async Task GetProfile()
        {
            UserDto = await _userDtoManagerService.GetWithNavigationProperties(await GetUserIdAsync());
            NewProfile = ObjectMapper.Map<UserDto, UpdateUserProfileRequestDto>(UserDto.User);
        }

        public async Task ResetPassword()
        {
            await InvokeAsync(async () =>
            {
                NewPassword.UserName = await GetUserNameAsync();
                await _userDtoManagerService.SetNewPasswordAsync(NewPassword);
            },ActionType.Reset,true);

            NewPassword = new NewUserPasswordDto();
        }

        public async Task UpdateProfile()
        {
            await InvokeAsync(async () =>
            {
                if (AvatarFile != null)
                {
                    //var uploadedFile = await _uploadService.UploadUserAvatar(AvatarFile);
                    //NewProfile.AvatarURL = uploadedFile.URL;
                }

                await _userDtoManagerService.UpdateProfile(NewProfile);
                _navigationManager.NavigateTo(_navigationManager.Uri,true);
                
            },ActionType.Update,true);
        }

        void OnProgress(UploadProgressArgs args, string name)
        {

            if (args.Progress == 100)
            {
                foreach (var file in args.Files)
                {
                    // Upload local file
                    fileName = file.Name;
                    AvatarFile =file;
                }
            }
        }
        private async void UploadFiles(IBrowserFile file)
        {
            await InvokeAsync(async () =>
            {
                AvatarFile = null;
                if (file.Size > BlazorSetting.Avatar_LENGTH_LIMIT)
                {
                    throw new FailedOperation(@L["FileSoBig"]);
                }

                //AvatarFile = file;
            }, ActionType.UploadFile);

        }

    }
}