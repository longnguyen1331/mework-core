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
        private IBrowserFile AvatarFile { get; set; }
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
                    var uploadedFile = await _uploadService.UploadUserAvatar(AvatarFile);
                    NewProfile.AvatarURL = uploadedFile.URL;
                }

                await _userDtoManagerService.UpdateProfile(NewProfile);
                _navigationManager.NavigateTo(_navigationManager.Uri,true);
                
            },ActionType.Update,true);
        }

      
        async Task OnChangeImageFile(InputFileChangeEventArgs e)
        {
            await InvokeAsync(async () =>
            {
                AvatarFile = null;
                AvatarFile = e.File;
                if (e.File.Size > BlazorSetting.Document_FILE_LENGTH_LIMIT)
                    throw new FailedOperation(@L["FileSoBig"]);
            }, ActionType.UploadFile, false);
        }

    }
}