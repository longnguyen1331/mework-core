using Blazorise;
using Contract;
using Contract.Genders;
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
    public partial class EditService
    {
        public ServiceDto Service = new ServiceDto();
        public CreateUpdateServiceDto NewService = new CreateUpdateServiceDto();
        public CreateUpdateServiceDto EditingService = new CreateUpdateServiceDto();
        public Guid? EditServiceId { get; set; }
        [Inject] IMessageService _messageService { get; set; }

        public List<ServiceTypeDto> ServiceTypes { get; set; } = new List<ServiceTypeDto>();
        public Guid? SelectedServiceTypeId;
        public Guid? ImageEditId;

        public Modal CreateModal;
        public Modal EditingModal;
        public string HeaderTitle = "Edit Service";
        public IEnumerable<string> Claims = new List<string>();
        public List<string> Tags = new List<string>();
        public IBrowserFile? ImageFile { get; set; }
        public string Base64EncodedImageData { set; get; }


        [Parameter]
        public string Id { get; set; }

        public EditService()
        {
        }
        
        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    HeaderTitle = L["EditService"];
                    await BuildServiceDto();
                    await GetServiceType();
                    StateHasChanged();
                }, ActionType.LoadData, false);
            }
        }
        public async Task GetServiceType()
        {
            ServiceTypes = await _serviceTypeService.GetListAsync();
        }
    

        public async Task UpdateService()
        {

            EditingService.Tags =string.Join(",", Tags);
            EditingService.ServiceTypeId = SelectedServiceTypeId;
            if (ImageEditId != EditingService.ImageId)
            {
                if (ImageFile is null)
                {
                    if (EditingService.ImageId == null || EditingService.ImageId.Value == Guid.Empty)
                    {
                        throw new FailedOperation(L["AppConfig.ImportLogoAndIcon"]);
                    }
                }

                if (ImageFile is not null)
                {
                    var fileId = await _uploadService.UploadImage1(ImageFile);
                    EditingService.ImageId = fileId;
                }
            }

            var result = await _serviceService.UpdateAsync(EditingService, EditServiceId.Value);
            if (result.IsSuccess)
            {
                NotifyMessage(NotificationSeverity.Success, "Edit Success", 4000);
            }
            else
            {
                NotifyMessage(NotificationSeverity.Error, result.Message, 4000);
            }
        }

        public async Task DeleteService(Guid id)
        {
            await InvokeAsync(async () =>
            {
                await _serviceService.DeleteAsync(id);
            }, ActionType.Delete, true);
        }

        public async Task ShowConfirmMessage(Guid id)
        {
            if (await _messageService.Confirm(L["Confirmation.Message"], L["Confirmation"]))
            {
                await DeleteService(id);
            }
        }

        public async Task BuildServiceDto()
        {
            EditingService = new CreateUpdateServiceDto();
            EditServiceId = null;
            if (!string.IsNullOrEmpty(Id))
            {
                HeaderTitle = L["EditService"];
                var result = await _serviceService.GetByIdAsync(Guid.Parse(Id));
                if (result.IsSuccess)
                {
                    Service = result.Data;
                    ImageEditId = null;
                    EditingService = ObjectMapper.Map<ServiceDto, CreateUpdateServiceDto>(Service);
                    EditServiceId = Service.Id;
                    SelectedServiceTypeId = EditingService.ServiceTypeId;
                    ImageEditId = Service.ImageId;

                    if (!string.IsNullOrEmpty(EditingService.Tags))
                    {
                        Tags = EditingService.Tags.Split(',').ToList();
                        EditingService.Tags = "";
                    }


                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            using (var response = await client.GetAsync(Service.ImageUrl))
                            {
                                byte[] imageBytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                                Base64EncodedImageData = Convert.ToBase64String(imageBytes);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Base64EncodedImageData = null;
                    }
                }
                else
                {
                    NotifyMessage(NotificationSeverity.Error, result.Message, 4000);
                }

            }
        }

        async Task OnChangeImageFile(InputFileChangeEventArgs e)
        {
            await InvokeAsync(async () =>
            {
                ImageFile = null;
                ImageFile = e.File;

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
            EditingService.Tags = "";
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

    }
}