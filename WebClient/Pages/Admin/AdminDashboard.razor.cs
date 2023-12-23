using Contract.Departments;
using Core.Enum;
using Radzen;
using WebClient.Components;
using DateRange = BlazorDateRangePicker.DateRange;

namespace WebClient.Pages.Admin
{
    public partial class AdminDashboard
    {
         public string HeaderTitle = "Admin Dashboard";

        public bool IsLoading { get; set; } = true;
        public bool IsLoadingChart { get; set; } = true;
        public Dictionary<string, DateRange> DateRanges { get; set; } = new Dictionary<string, DateRange>();
        public (DateTimeOffset? StartDay, DateTimeOffset? EndDay) Timeline = (null, null);
        
        public RZModel ScheduleFileView { get; set;}
        public RZModel ContentFileView { get; set;}
        
        public List<DepartmentDto> SelectionHierarchicalDepartments { get; set; } = new List<DepartmentDto>();
        public List<DepartmentDto> Departments { get; set; } = new List<DepartmentDto>();
        public TaskStatusType? DeptTaskStatus { get; set; } = TaskStatusType.Completed;
        
        public IEnumerable<string> ChartFills { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
           
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                HeaderTitle = L["MyDashboard"];
                IsLoading = false;
                StateHasChanged();
            }
        }
        
        
        
        
        
        
        public void TransformHierarchicalTreeOfDepartments(DepartmentDto root, int level,
            List<DepartmentDto> hierarchicalIssuingAgencies)
        {

            var icon = SetSpaceByLevel(level);
            root.Name = icon + root.Name;
            hierarchicalIssuingAgencies.Add(root);
            var childNode = Departments.Where(x => x.ParentCode == root.Id);

            foreach (var item in childNode)
            {
               
                TransformHierarchicalTreeOfDepartments(item, level + 1, hierarchicalIssuingAgencies);
            }

        }
        
        private static string? SetSpaceByLevel(int level)
        {
            var space = "";
            for (int i = 0; i < level; i++)
            {
                space += "-";
            }

            return space;
        }
        
        void OnSeriesClick(SeriesClickEventArgs args)
        {
           
        }
        
 
        
    }
    
}