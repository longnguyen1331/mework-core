using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorDateRangePicker;
using Microsoft.AspNetCore.Components;

namespace WebClient.Pages.Admin
{
    public partial class Index 
    {
        private int number = 10;
        private DateTimeOffset? StartDate { get; set;}
        
        private DateTimeOffset? EndDate { get; set;}
        
        DateTime? value = DateTime.Now;


        private Dictionary<string, DateRange> DateRanges { get; set; } = new Dictionary<string, DateRange>();
        public Index()
        {
           
        }

        protected override async Task OnInitializedAsync()
        {
            DateRanges = await GetDateRangePickers();
        }

        private async Task call()
        {
           // await  _userManagerService.test();
        }
        
        void OnChange(DateTime? value, string name, string format)
        {
            
            
        }
        
        void Change(string text)
        {
            _isOpen = !_isOpen; 
           Console.WriteLine("aaaa");
        }
   
    }
}