using System;
using Microsoft.AspNetCore.Components;

namespace WebClient.Components
{
    public partial class RZEmptyValidation
    {
        
        [Parameter]
        public Guid Item { get; set; }
        public string Message { get; set;}
        
        public bool First { get; set; }


        protected override void OnParametersSet()
        {
            Console.WriteLine("abc");
            base.OnParametersSet();
        }
    }
}