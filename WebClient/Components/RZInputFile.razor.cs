using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace WebClient.Components
{
    public partial class RZInputFile: ComponentBase
    {
        [Parameter]
        public EventCallback<InputFileChangeEventArgs> Event { get; set; } =
            new EventCallback<InputFileChangeEventArgs>();
        
        [Parameter]
        public string Text { get; set;}
        
        [Parameter]
        public bool IsMultiple { get; set; }
        
        [Parameter]
        public string Accept { get; set; }
        
        [Parameter]
        public string Icon { get; set; }
        
        [Parameter]
        public string Style { get; set;}
        

        private string _fileName = "";
        private Guid  FileID  = Guid.NewGuid();

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }

        private async Task OnChangeFileAtEditModal(InputFileChangeEventArgs e)
        {
            await Event.InvokeAsync(e);
            if (IsMultiple)
            {
                foreach (var item in e.GetMultipleFiles())
                {
                    _fileName +="-"+ item.Name +"<br/>";
                }
            }
            else
            {
                _fileName = e.File.Name;

            }
        }
    }
}