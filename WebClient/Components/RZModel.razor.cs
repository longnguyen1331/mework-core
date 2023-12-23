using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Nito.AsyncEx;

namespace WebClient.Components
{
    public partial class RZModel : ComponentBase
    {
        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public string Width { get; set; }
        [Parameter] public string Height { get; set; }
        [Parameter] public string Style { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public bool ShowTitle { get; set; }


        public RZModel()
        {
          
        }

        protected override async Task OnParametersSetAsync()
        {
            DialogService.Refresh();
        }

        public void Refresh()
        {
            DialogService.Refresh();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            StateHasChanged();
        }
    }
}