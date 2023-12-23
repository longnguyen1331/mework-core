using Blazorise;
using Contract.Services;
using Contract.ServiceTypes;
using Core.Enum;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Radzen;
using WebClient.Exceptions;
using WebClient.Setting;

namespace WebClient.Pages.Admin.Services
{
    public partial class CreateService
    {
        public ServiceDto Service = new ServiceDto();
        public CreateUpdateServiceDto NewService = new CreateUpdateServiceDto();
        public CreateUpdateServiceDto CreatingService = new CreateUpdateServiceDto();
        public Guid? CreateServiceId { get; set; }
        [Inject] IMessageService _messageService { get; set; }

        public List<ServiceTypeDto> ServiceTypes { get; set; } = new List<ServiceTypeDto>();
        public Guid? SelectedServiceTypeId;
        public Guid? ImageEditId;

        public Modal CreateModal;
        public Modal CreatingModal;
        public string HeaderTitle = "Create Service";
        public IEnumerable<string> Claims = new List<string>();
        public List<string> Tags = new List<string>();
        public IBrowserFile? ImageFile { get; set; }
        public string Base64EncodedImageData { set; get; }


        [Parameter]
        public string Id { get; set; }
        public CreateService()
        {
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    HeaderTitle = L["CreateService"];
                    CreatingService = new CreateUpdateServiceDto();
                    await GetServiceType();
                    StateHasChanged();
                }, ActionType.LoadData, false);
            }
        }
        public async Task GetServiceType()
        {
            ServiceTypes = await _serviceTypeService.GetListAsync();
        }


        public async Task OnCreateService()
        {
            try
            {
                CreatingService.Tags = string.Join(",", Tags);
                CreatingService.ServiceTypeId = SelectedServiceTypeId;

                var result = await _serviceService.CreateAsync(input: CreatingService);
                if (result.IsSuccess)
                {
                    NotifyMessage(NotificationSeverity.Success, "Create Success", 4000);
                    BackToList();
                }
                else
                {
                    NotifyMessage(NotificationSeverity.Error, result.Message, 4000);
                }
            }
            catch (Exception ex)
            {
                NotifyMessage(NotificationSeverity.Error, ex.Message, 4000);
            }

        }

        async Task OnChangeImageFile(InputFileChangeEventArgs e)
        {
            await InvokeAsync(async () =>
            {
                ImageFile = null;
                ImageFile = e.File;
                var fileId = await _uploadService.UploadImage1(ImageFile);
                CreatingService.ImageId = fileId;

                using (var ms = new MemoryStream())
                {
                    await e.File.OpenReadStream(25214400).CopyToAsync(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    var bytes = ms.ToArray();
                    Base64EncodedImageData = Convert.ToBase64String(bytes);
                }

                if (e.File.Size > BlazorSetting.Document_FILE_LENGTH_LIMIT)
                    throw new FailedOperation(@L["FileSoBig"]);
            }, ActionType.UploadFile, false);
        }

        public async Task TagOnChange(string value)
        {
            if (!Tags.Any(x => x.Trim().ToLower().Equals(value.Trim().ToLower())))
                Tags.Add(value);
            CreatingService.Tags = "";
            await JSRuntime.InvokeVoidAsync("eval", $@"document.getElementsByName(""txtarea_tag"")[0].focus()");
        }

        public void RemoveTag(string value)
        {
            Tags.Remove(value);
        }


        async Task BackToList()
        {
            _navigationManager.NavigateTo($"services");
        }

        void SetValueServiceType()
        {
            CreatingService.ServiceTypeId = SelectedServiceTypeId;
        }
    }
}