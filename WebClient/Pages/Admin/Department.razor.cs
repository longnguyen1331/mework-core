using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Contract.Departments;
using Core.Enum;
using Core.Extension;
using Microsoft.AspNetCore.Components;
using Radzen;
using WebClient.LanguageResources;

namespace WebClient.Pages.Admin
{
    public partial class Department
    {
        public List<DepartmentDto> HierarchicalDepartments = new List<DepartmentDto>();
        public List<DepartmentDto> Departments = new List<DepartmentDto>();

        
        public CreateUpdateDepartmentDto NewDepartment = new CreateUpdateDepartmentDto();
        public CreateUpdateDepartmentDto EditingDepartment = new CreateUpdateDepartmentDto();
        public Guid EditingDepartmentId { get; set; }
         [Inject]  IMessageService _messageService { get; set; }


        public Modal CreateModal;
        public Modal EditingModal;
        public string HeaderTitle = "Department";
     
        

        public Department()
        {
            
        }
        
       

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    HeaderTitle = L["Department"];
                    await GetDepartments();
                    StateHasChanged();
                }, ActionType.LoadData, false);
            }
        }

        public async Task GetDepartments()
        {
            HierarchicalDepartments = await _departmentService.GetListAsync();

            Departments = HierarchicalDepartments.Clone();
            foreach (var item in HierarchicalDepartments)
            {
                var childDepartments = 
                    HierarchicalDepartments.Where(x => x.ParentCode == item.Id).ToList();
                item.ChildDepartment = childDepartments;
            }

            HierarchicalDepartments = HierarchicalDepartments.Where(x => x.ParentCode == null).ToList();

        }



        
        

        public async Task CreateDepartment()
        {
            await InvokeAsync(async () =>
            {
                await _departmentService.CreateAsync(input: NewDepartment);
                HideNewModal();
                await GetDepartments();
            }, ActionType.Create, true);
        }

        public async Task UpdateDepartment()
        {
            await InvokeAsync(async () =>
            {
                await _departmentService.UpdateAsync(EditingDepartment, EditingDepartmentId);
                HideEditModal();    
                await GetDepartments();
            },ActionType.Update,true);
            
        }

        public async Task DeleteDepartment(Guid id)
        {
            await InvokeAsync(async () =>
            {
                await _departmentService.DeleteAsync(id);
                HideEditModal();
                await GetDepartments();
            },ActionType.Delete,true);
            
        }

        public async Task ShowConfirmMessage(object value)
        {
            var department = (DepartmentDto)value;

            if (await _messageService.Confirm(L["Confirmation.Message"], L["Confirmation"]))
            {
                await DeleteDepartment(department.Id);
            }
        }

        public void ShowNewModal()
        {
            NewDepartment = new CreateUpdateDepartmentDto();
            CreateModal.Show();
        }

        public void HideNewModal()
        {
            CreateModal.Hide();
        }

        public Task ShowEditingModal(object value)
        {
            var department = (DepartmentDto)value;

            EditingDepartment = new CreateUpdateDepartmentDto();
            EditingDepartment = ObjectMapper.Map<DepartmentDto, CreateUpdateDepartmentDto>(department);
            EditingDepartmentId = department.Id;
            return EditingModal.Show();
        }

        public void HideEditModal()
        {
            EditingModal.Hide();
        }
        
        

        void OnChangeNewDepartment(TreeEventArgs args)
        {
            var department = (DepartmentDto) args.Value;
            NewDepartment.ParentCode = department.Id;
        }
       
        void OnChangeUpdateDepartment(TreeEventArgs args)
        {
            
            var department = (DepartmentDto) args.Value;
            
            if (EditingDepartmentId != department.Id)
            {
                if (IsChildOfAssignmentDepartment(EditingDepartmentId,department))
                {
                    NotifyMessage(NotificationSeverity.Warning, "You can't choose its child",4000);
                }
                else
                {
                    EditingDepartment.ParentCode = department.Id;
                }
            }
        }


        bool IsChildOfAssignmentDepartment(Guid assignmentId,DepartmentDto dto)
        {
            var childDepartments = new List<DepartmentDto>();
            var department = Departments.FirstOrDefault(x => x.Id == assignmentId);
            GetAllChildOfDepartment(childDepartments, department);

            var item = childDepartments.FirstOrDefault(x => x.Id == dto.Id);

            if (item != null) return true;

            return false;

        }       

        void GetAllChildOfDepartment(List<DepartmentDto> childs,DepartmentDto dto)
        {
            var items = Departments.Where(x => x.ParentCode == dto.Id);
            childs.AddRange(items);
            foreach (var item in items)
            {
                GetAllChildOfDepartment(childs, item);
            }
            
        }
        
        
        
        
        
        
        
        
   
       
    }
    

}