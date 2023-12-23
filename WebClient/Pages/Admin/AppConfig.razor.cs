using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Contract.AppConfigs;
using Core.Enum;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using WebClient.Components;
using WebClient.Exceptions;
using WebClient.Setting;

namespace WebClient.Pages.Admin
{
    public partial class AppConfig
    {
        public List<AppConfigDto> AppConfigs = new List<AppConfigDto>();
        public CreateUpdateAppConfigDto NewAppConfig = new CreateUpdateAppConfigDto();
        public CreateUpdateAppConfigDto EditingAppConfig = new CreateUpdateAppConfigDto();
        public Guid EditingUnitId { get; set; }
        [Inject] IMessageService _messageService { get; set; }


        public RZModel CreateModal;
        public RZModel EditingModal;

        public IBrowserFile? NewLogoFile { get; set; }
        public IBrowserFile? NewIconFile { get; set; }

        public IBrowserFile? EditingLogoFile { get; set; }
        public IBrowserFile? EditingIconFile { get; set; }
        
        public bool IsVisible { get; set; }

        public string HeaderTitle = "App Config";


        public AppConfig()
        {
        }


        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    HeaderTitle = L["AppConfig"];
                    await GetAppConfigs();
                    StateHasChanged();
                }, ActionType.LoadData, false);
            }
        }

        
        public async Task GetAppConfigs()
        {
           var configResponse  = await _appConfigService.GetListAsync();

            if (configResponse.IsSuccess)
            {
                AppConfigs = configResponse.Data;
            }
        }
        

        public async Task CreateAppConfig()
        {
            await InvokeAsync(async () =>
            {
                if (NewIconFile is null || NewLogoFile is null)
                {
                    throw new FailedOperation(L["AppConfig.ImportLogoAndIcon"]);
                }

                var files = new List<IBrowserFile>();
                files.Add(NewIconFile);
                files.Add(NewLogoFile);
                var fileIds = await  _uploadService.UploadImages(files);
                NewAppConfig.IconId = fileIds.FirstOrDefault();
                NewAppConfig.LogoId = fileIds.LastOrDefault();
                
                await _appConfigService.CreateAsync(input: NewAppConfig);
                HideNewModal();
                await GetAppConfigs();
            }, ActionType.Create, true);
        }
        
        public async Task UpdateAppConfig()
        {
            await InvokeAsync(async () =>
            {
                if (EditingIconFile is null && EditingLogoFile is null)
                {
                    if (EditingAppConfig.LogoId == Guid.Empty || EditingAppConfig.IconId == Guid.Empty)
                    {
                        throw new FailedOperation(L["AppConfig.ImportLogoAndIcon"]);
                    }
                }
               
                if (EditingIconFile is not null && EditingLogoFile is not null)
                {
                    var fileIds = new List<Guid>();
                    var files = new List<IBrowserFile>();
                        files.Add(EditingIconFile);
                        files.Add(EditingLogoFile);
                        fileIds = await  _uploadService.UploadImages(files);
                        EditingAppConfig.IconId = fileIds.FirstOrDefault();
                        EditingAppConfig.LogoId = fileIds.LastOrDefault();
                }else if (EditingIconFile is not null)
                {
                 var fileId =  await  _uploadService.UploadImage1(EditingIconFile);
                 EditingAppConfig.IconId = fileId;
                }else if (EditingLogoFile is not null)
                {
                    var fileId =  await  _uploadService.UploadImage1(EditingLogoFile);
                    EditingAppConfig.LogoId = fileId;
                }
               
       
                
                await _appConfigService.UpdateAsync(input: EditingAppConfig,EditingUnitId);
                HideNewModal();
                await GetAppConfigs();
            }, ActionType.Update, true);
        }
        
        

        public async Task DeleteAppConfig(Guid id)
        {
            await InvokeAsync(async () =>
            {
                await _appConfigService.DeleteAsync(id);
                HideEditModal();
                await GetAppConfigs();
            }, ActionType.Delete, true);
        }

        public async Task ShowConfirmMessage(Guid id)
        {
            if (await _messageService.Confirm(L["Confirmation.Message"], L["Confirmation"]))
            {
                await DeleteAppConfig(id);
            }
        }

        public async  void ShowNewModal()
        {
            NewAppConfig = new CreateUpdateAppConfigDto();
            NewIconFile = null;
            NewLogoFile = null;
            await  CreateModal.ShowModel();
          
        
        }

        public void HideNewModal()
        {
            CreateModal.HideModel();
        }


        public Task ShowEditingModal(AppConfigDto dto)
        {
            EditingAppConfig = new CreateUpdateAppConfigDto();
            EditingAppConfig = ObjectMapper.Map<AppConfigDto, CreateUpdateAppConfigDto>(dto);
            EditingUnitId = dto.Id;
            EditingIconFile = null;
            EditingLogoFile = null;
            return EditingModal.ShowModel();
        }

        public void HideEditModal()
        {
            EditingModal.HideModel();
        }

        public void OnChangedEnableNotification(bool value)
        {
            NewAppConfig.EnableNotificationByEmail = value;
            Console.WriteLine(value);
        }
        
        async Task OnChangeIconFileAtNewModal(InputFileChangeEventArgs e)
        {
            await InvokeAsync(async () =>
            {
                NewIconFile = null;
                NewIconFile = e.File;
                if (e.File.Size > BlazorSetting.Document_FILE_LENGTH_LIMIT)
                    throw new FailedOperation(@L["FileSoBig"]);
            },ActionType.UploadFile,false);
        }
        async Task OnChangeLogoFileAtNewModal(InputFileChangeEventArgs e)
        {
            await InvokeAsync(async () =>
            {
                NewLogoFile = null;
                NewLogoFile = e.File;
                if (e.File.Size > BlazorSetting.Document_FILE_LENGTH_LIMIT)
                    throw new FailedOperation(@L["FileSoBig"]);
            },ActionType.UploadFile,false);
        }

        
        async Task OnChangeIconFileAtEditingModal(InputFileChangeEventArgs e)
        {
            await InvokeAsync(async () =>
            {
                EditingIconFile = null;
                EditingIconFile = e.File;
                if (e.File.Size > BlazorSetting.Document_FILE_LENGTH_LIMIT)
                    throw new FailedOperation(@L["FileSoBig"]);
            },ActionType.UploadFile,false);
        }
        async Task OnChangeLogoFileAtEditingModal(InputFileChangeEventArgs e)
        {
            await InvokeAsync(async () =>
            {
                EditingLogoFile = null;
                EditingLogoFile = e.File;
                if (e.File.Size > BlazorSetting.Document_FILE_LENGTH_LIMIT)
                    throw new FailedOperation(@L["FileSoBig"]);
            },ActionType.UploadFile,false);
        }
        
        public async Task OnChangeSelectBar(bool value,Guid id)
        {
            IsVisible = true;
            var flag = false;
            await InvokeAsync(async () =>
            {
                if (value)
                {
                    await _appConfigService.ApplyConfig(id);
                }
                else
                {
                    await _appConfigService.SwitchOffConfig(id);
                }
                NotifyMessage(NotificationSeverity.Success,"Successful!. Reload After 2s",2000);
            },ActionType.Update,false);

           
              await Task.Delay(2000);
            _navigationManager.NavigateTo("app-config",true);

        }
        
        
        
    }
}