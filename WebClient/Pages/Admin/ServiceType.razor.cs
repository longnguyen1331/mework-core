using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise;
using Contract.ServiceTypes;
using Core.Enum;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using WebClient.Exceptions;
using WebClient.LanguageResources;
using WebClient.Setting;

namespace WebClient.Pages.Admin
{
    public partial class ServiceType
    {
        public List<ServiceTypeDto> ServiceTypes = new List<ServiceTypeDto>();
        public CreateUpdateServiceTypeDto NewServiceType = new CreateUpdateServiceTypeDto();
        public CreateUpdateServiceTypeDto EditingServiceType = new CreateUpdateServiceTypeDto();
        public Guid EditServiceTypeId { get; set; }
        [Inject]  IMessageService _messageService { get; set; }

        public Modal CreateModal;
        public Modal EditingModal;
        public string HeaderTitle = "Service Type";
        public IEnumerable<string> Claims = new List<string>();
        public IBrowserFile? NewImageFile { get; set; }
        public IBrowserFile? EditingImageFile { get; set; }

        public ServiceType()
        {
        }
        protected override async void OnAfterRender(bool firstRender)
        {

            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    HeaderTitle = L["ServiceType"];
                    await GetServiceTypes();
                    StateHasChanged();
                }, ActionType.LoadData, false);
            }
        }

        public async Task GetServiceTypes()
        {
            ServiceTypes = await _serviceTypeService.GetListAsync();
        }

        public async Task CreateServiceType()
        {
            await InvokeAsync(async () =>
            {

                if (NewImageFile is null)
                {
                    throw new FailedOperation(L["AppConfig.ImportLogoAndIcon"]);
                }

                var files = new List<IBrowserFile>();
                files.Add(NewImageFile);
                var fileIds = await _uploadService.UploadImages(files);
                NewServiceType.ImageId = fileIds.FirstOrDefault();

                await _serviceTypeService.CreateAsync(input: NewServiceType);
                HideNewModal();
                await GetServiceTypes();
            }, ActionType.Create, true);
        }

        public async Task UpdateServiceType()
        {
            await InvokeAsync(async () =>
            {
                if (EditingImageFile is null)
                {
                    if (EditingServiceType.ImageId == null || EditingServiceType.ImageId.Value  == Guid.Empty)
                    {
                        throw new FailedOperation(L["AppConfig.ImportLogoAndIcon"]);
                    }
                }

                if (EditingImageFile is not null)
                {
                    var fileId = await _uploadService.UploadImage1(EditingImageFile);
                    EditingServiceType.ImageId = fileId;
                }

                await _serviceTypeService.UpdateAsync(EditingServiceType, EditServiceTypeId);
                HideEditModal();
                await GetServiceTypes();
            },ActionType.Update,true);
            
        }

        public async Task DeleteServiceType(Guid id)
        {
            await InvokeAsync(async () =>
            {
                await _serviceTypeService.DeleteAsync(id);
                HideEditModal();
                await GetServiceTypes();
            },ActionType.Delete,true);
            
        }

        public async Task ShowConfirmMessage(Guid id)
        {
            if (await _messageService.Confirm(L["Confirmation.Message"], L["Confirmation"]))
            {
                await DeleteServiceType(id);
            }
        }

        public void ShowNewModal()
        {
            NewServiceType = new CreateUpdateServiceTypeDto();
            CreateModal.Show();
        }

        public void HideNewModal()
        {
            CreateModal.Hide();
        }

        public Task ShowEditingModal(ServiceTypeDto serviceType)
        {
            EditingServiceType = new CreateUpdateServiceTypeDto();
            EditingServiceType = ObjectMapper.Map<ServiceTypeDto, CreateUpdateServiceTypeDto>(serviceType);
            EditServiceTypeId = serviceType.Id;
            return EditingModal.Show();
        }

        public void HideEditModal()
        {
            EditingModal.Hide();
        }
        async Task OnChangeImageFileAtNewModal(InputFileChangeEventArgs e)
        {
            await InvokeAsync(async () =>
            {
                NewImageFile = null;
                NewImageFile = e.File;
                if (e.File.Size > BlazorSetting.Document_FILE_LENGTH_LIMIT)
                    throw new FailedOperation(@L["FileSoBig"]);
            }, ActionType.UploadFile, false);
        }
        async Task OnChangeImageFileAtEditingModal(InputFileChangeEventArgs e)
        {
            await InvokeAsync(async () =>
            {
                EditingImageFile = null;
                EditingImageFile = e.File;
                if (e.File.Size > BlazorSetting.Document_FILE_LENGTH_LIMIT)
                    throw new FailedOperation(@L["FileSoBig"]);
            }, ActionType.UploadFile, false);
        }

    }
}