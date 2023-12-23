using Contract.AppHistories;
using Core.Enum;
using Radzen;

namespace WebClient.Pages.Admin
{
    public partial class AppHistory
    {
        public List<AppHistoryDto> AppHistories = new List<AppHistoryDto>();

        public string HeaderTitle = "App history";

        public bool IsLoading { get; set; } = true;
        public int TotalItem { set; get; } = 0;
        public int DefaultPageSize { set; get; } = 20;

        public AppHistory()
        {
        }

        private AppHistoryFilterPagingDto Filter = new AppHistoryFilterPagingDto();

        protected override async void OnAfterRender(bool firstRender)
        {
            Filter.Take = DefaultPageSize;

            if (firstRender)
            {
                IsLoading = true;

                await InvokeAsync(async () =>
                {
                    HeaderTitle = L["AppHistory"];
                    await GetAppHistories();

                    StateHasChanged();
                }, ActionType.LoadData, false);

                IsLoading = false;
            }
        }

        public async Task GetAppHistories(LoadDataArgs? args = null)
        {
            IsLoading = true;

            if (args != null)
            {
                Filter.Skip = args.Skip ?? 0;
                Filter.Take = args.Top ?? DefaultPageSize;

                if (args.Filters != null)
                {
                    Filter.Functions = null;
                    Filter.IpAddress = null;
                    Filter.Operation = null;
                    Filter.FullName = null;
                    Filter.Date = null;

                    foreach (var filter in args.Filters)
                    {
                        if (nameof(Filter.Functions) == filter.Property)
                            Filter.Functions = (string)filter.FilterValue;

                        if (nameof(Filter.IpAddress) == filter.Property)
                            Filter.IpAddress = (string)filter.FilterValue;

                        if (nameof(Filter.Operation) == filter.Property)
                            Filter.Operation = (string)filter.FilterValue;

                        if (nameof(Filter.FullName) == filter.Property)
                            Filter.FullName = (string)filter.FilterValue;

                        if (nameof(Filter.Date) == filter.Property)
                            Filter.Date = (DateTime)filter.FilterValue;
                    }
                }
            }

            var result = await _appHistoryService.GetListAsync(Filter);

            await Task.Yield();

            if (result.IsSuccess == true)
            {
                AppHistories = result.Data.Result;
                TotalItem = result.Data.TotalItem;
            }
            else
                NotifyMessage(NotificationSeverity.Error, result.Message, 2000);

            IsLoading = false;
        }
    }
}