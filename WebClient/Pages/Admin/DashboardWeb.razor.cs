using BlazorDateRangePicker;
using Core.Enum;

namespace WebClient.Pages.Admin
{
    public partial class DashboardWeb
    {
        public string HeaderTitle = "Overview";
        public bool IsLoading { get; set; } = true;
        public bool IsLoadingChart { get; set; } = true;
        public Dictionary<string, DateRange> DateRanges { get; set; } = new Dictionary<string, DateRange>();
        public (DateTimeOffset? StartDay, DateTimeOffset? EndDay) Timeline = (null, null);

        protected override async Task OnInitializedAsync()
        {
         
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                    {
                        HeaderTitle = L["Overview"];
                        IsLoading = false;
                        StateHasChanged();
                    }
                    , ActionType.LoadData);
               
            }
        }


    }
}