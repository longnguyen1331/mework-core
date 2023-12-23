using Blazorise;
using Contract.Services;
using Core.Enum;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace WebClient.Pages.Admin.Services
{
    public partial class Service
    {
        public List<ServiceDto> Services = new List<ServiceDto>();
        public CreateUpdateServiceDto NewService = new CreateUpdateServiceDto();
        public CreateUpdateServiceDto EditingService = new CreateUpdateServiceDto();
        public Guid EditServiceId { get; set; }
        [Inject]  IMessageService _messageService { get; set; }

        public string HeaderTitle = "Service";

        public Service()
        {
        }
        protected override async void 
            OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    HeaderTitle = L["Service"];
                    await GetServices();
                    StateHasChanged();
                }, ActionType.LoadData, false);
            }
        }
     
        public async Task GetServices()
        {
            var result = await _serviceService.GetListAsync();
            if (result.IsSuccess)
                Services = result.Data;
        }


        public async Task DeleteService(Guid id)
        {
            var result = await _serviceService.DeleteAsync(id);
            if(result.IsSuccess)
            {
                await GetServices();
                NotifyMessage(NotificationSeverity.Success, "Delete Success", 2000);
            }
            else
            {
                NotifyMessage(NotificationSeverity.Error, result.Message, 2000);
            }
        }

        public async Task ShowConfirmMessage(Guid id)
        {
            if (await _messageService.Confirm(L["Confirmation.Message"], L["Confirmation"]))
            {
                await DeleteService(id);
            }
        }
        async Task GoToEditPage(string? id)
        {
            _navigationManager.NavigateTo($"services/{id}");
        }

        async Task GoToCreatePage()
        {
            _navigationManager.NavigateTo($"services/create");
        }
    }
}