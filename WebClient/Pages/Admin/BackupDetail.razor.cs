using Contract.BackupDetails;
using Core.Enum;
using Microsoft.JSInterop;
using Radzen;

namespace WebClient.Pages.Admin
{
    public partial class BackupDetail
    {
        public List<BackupDetailDto> BackupDetails = new List<BackupDetailDto>();
        public string HeaderTitle = "BackupDetail";
        public int TotalItem { set; get; } = 0;
        public int DefaultPageSize { set; get; } = 20;
        private BackupDetailFilterPagingDto Filter = new BackupDetailFilterPagingDto();
        public bool IsLoading { get; set; } = true;
        public BackupDetail()
        {
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                IsLoading = true;
                await InvokeAsync(async () =>
                {
                    HeaderTitle = L["BackupDetail"];
                    await GetBackupDetails();
                    StateHasChanged();
                }, ActionType.LoadData, false);
                IsLoading = false;
            }
        }

        public async Task GetBackupDetails(LoadDataArgs? args = null)
        {
            IsLoading = true;

            if (args != null)
            {
                Filter.Skip = args.Skip ?? 0;
                Filter.Take = args.Top ?? DefaultPageSize;

                if (args.Filters != null)
                {
                  
                }
            }

            var response = await _backupDetailService.GetListAsync(Filter);
            if (response.IsSuccess)
            {
                BackupDetails = response.Data.Result;
                TotalItem = response.Data.TotalItem;
            }
            else
                NotifyMessage(NotificationSeverity.Error, response.Message, 2000);

            IsLoading = false;

        }

        public async Task Download(BackupDetailDto data)
        {
            IsLoading = true;
            string fullPath = $"{Configuration["RemoteServices:BaseStaticFileUrl"]}" + data.FullFilePath.Replace("\\", "/");
            await JS.InvokeVoidAsync("downloadDifferentDomain", fullPath, data.FullFilePath.Replace("Backups/", ""));
            IsLoading = false;
        }
    }
}