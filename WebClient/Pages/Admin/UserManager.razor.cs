using Blazorise;
using Contract.Departments;
using Contract.Identity.RoleManager;
using Contract.Identity.UserManager;
using Contract.Positions;
using Core.Enum;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using WebClient.Exceptions;
using WebClient.Helper;

namespace WebClient.Pages.Admin
{
    public partial class UserManager
    {
        [Inject] IMessageService _messageService { get; set; }
        public List<UserWithNavigationPropertiesDto> Users { get; set; } = new List<UserWithNavigationPropertiesDto>();
        public CreateUserDto NewUser { get; set; } = new CreateUserDto();
        public CreateUserDto UserExcel { get; set; } = new CreateUserDto();
        public UpdateUserDto EditingUser { get; set; } = new UpdateUserDto();
        public List<RoleDto> Roles = new List<RoleDto>();
        public List<string> RoleNames = new List<string>();
        public List<string> SelectedRoles = new List<string>();

        public List<PositionDto> Positions { get; set; } = new List<PositionDto>();
        public Guid? SelectedPositionId { get; set; }
        public IEnumerable<DepartmentDto> HierarchicalDepartments { get; set; } = new List<DepartmentDto>();
        public List<DepartmentDto> Departments { get; set; } = new List<DepartmentDto>();

        public IEnumerable<object> SelectedDepartments = new List<DepartmentDto>();


        public Guid EditingUserId { get; set; }

        public Modal CreateModal = new Modal();
        public Modal EditingModal = new Modal();
        public Modal ImportFileModal = new Modal();
       
        public IBrowserFile? EnclosedFile { get; set; }

        public UserValidatorExcel UserValidatorExcel { get; set; } = new UserValidatorExcel();

        public string HeaderTitle = "User";
        public bool IsLoading { get; set; } = true;
        public bool TriggeredWithoutFile { get; set; } = false;
        
        

        public UserManager()
        {
        }


        protected override async Task OnInitializedAsync()
        {
        
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    await GetUsers();
                    await GetRoles();
                    await GetPositions();
                    await GetDepartments();
                    IsLoading = false;
                    StateHasChanged();
                }, ActionType.LoadData, false);


            }
        }
    
        public async Task GetPositions()
        {
            Positions = await _positionService.GetListAsync();
        }

        public async Task GetDepartments()
        {
            Departments = await _departmentService.GetListAsync();
            HierarchicalDepartments = Departments;
            foreach (var item in HierarchicalDepartments)
            {
                var childDepartments = 
                    HierarchicalDepartments.Where(x => x.ParentCode == item.Id).ToList();
                item.ChildDepartment = childDepartments;
            }
            HierarchicalDepartments = HierarchicalDepartments.Where(x => x.ParentCode == null);
            
        }
        
        public async Task GetUsers()
        {
            Users = await _userManagerService.GetListWithNavigationAsync();
        }

        public async Task GetRoles()
        {
            Roles = await _roleManagerService.GetListAsync();
            RoleNames = Roles.Select(x => x.Name).ToList();
        }

        public async Task CreateUser()
        {
            await InvokeAsync(async () =>
            {
                NewUser.UserName = NewUser.PhoneNumber;
                NewUser.PositionId = SelectedPositionId;

                NewUser.DepartmentIds = (SelectedDepartments.OfType<DepartmentDto>())
                    .Where(x=>x.ChildDepartment.Count == 0)
                    .Select(x=>x.Id).ToList();
                await _userManagerService.CreateUserWithNavigationPropertiesAsync(NewUser);
                HideNewModal();
                await GetUsers();
            }, ActionType.Create, true);
        }

        public async Task UpdateUser()
        {
            await InvokeAsync(async () =>
            {
                EditingUser.UserName = EditingUser.PhoneNumber;
                EditingUser.PositionId = SelectedPositionId;

                EditingUser.DepartmentIds = (SelectedDepartments.OfType<DepartmentDto>())
                    .Where(x=>x.ChildDepartment.Count == 0)
                    .Select(x=>x.Id).ToList();
                
                await _userManagerService.UpdateUserWithNavigationPropertiesAsync(EditingUser, EditingUserId);
                await GetUsers();
                await HideEditingModal();
                
            }, ActionType.Update, true);
        }

        public async Task DeleteUser(Guid id)
        {
            await InvokeAsync(async () =>
            {
                await _userManagerService.DeleteAsync(id);
                await GetUsers();
            },ActionType.Delete,true );
        }
        
        
        public async Task ShowConfirmMessage(Guid id)
        {
            if ( await _messageService.Confirm( L["Confirmation.Message"], L["Confirmation"] ) )
            {
                await DeleteUser(id);
            }
        }


        public void ShowNewModal()
        {
            NewUser = new CreateUserDto();
            SelectedRoles = new List<string>();
            SelectedPositionId = null;
            SelectedDepartments = new List<DepartmentDto>();

            CreateModal.Show();
        }
        public async Task PushNotificationTest()
        {
            //var savedToken = await localStorage.GetItemAsync<string>("firebase-token");
            //List<string> tokens = new List<string>();
            //tokens.Add(savedToken);
            //_fcmHelper.SendFireBaseNotification(tokens, new
            //{
            //    notification = new {title = "test", body = "day la body"},
            //});
        }

        public Task ShowImportExcelFileModal()
        {
            EnclosedFile = null;
            UserExcel = new CreateUserDto();
            UserValidatorExcel = new UserValidatorExcel();
            TriggeredWithoutFile = false;
            return ImportFileModal.Show();
        }

        public void HideImportExcelFileModal()
        {
            ImportFileModal.Hide();
        }

        public void HideNewModal()
        {
            NewUser = new CreateUserDto();
            CreateModal.Hide();
        }
        
     

        public void ShowEditingModal(UserWithNavigationPropertiesDto userWithNavigationPropertiesDto)
        {
            
            UpdateSelectedDepartments(userWithNavigationPropertiesDto);
            
            EditingUser = new UpdateUserDto();
            SelectedRoles = new List<string>();
            SelectedPositionId = null;

            SelectedRoles = userWithNavigationPropertiesDto.RoleNames;
            SelectedPositionId = userWithNavigationPropertiesDto.Position?.Id;
            EditingUserId = userWithNavigationPropertiesDto.User.Id;
            EditingUser = ObjectMapper.Map<UserDto, UpdateUserDto>(userWithNavigationPropertiesDto.User);
            EditingUser.Roles = userWithNavigationPropertiesDto.RoleNames;
            EditingModal.Show();
        }

        private void UpdateSelectedDepartments(UserWithNavigationPropertiesDto userWithNavigationPropertiesDto)
        {
            var tempt = new List<DepartmentDto>();
            foreach (var item in userWithNavigationPropertiesDto.Departments)
            {
                tempt.Add(Departments.FirstOrDefault(x => x.Id == item.Id));
            }

            SelectedDepartments = tempt;
        }


        public BadgeStyle ChooseColorByNumber(int i)
        {
            
            
            switch (i)
            {
                case 1:
                {
                    return BadgeStyle.Info;
                }
                case 2:
                {
                    return BadgeStyle.Primary;
                }
                case 3:
                {
                    return BadgeStyle.Success;
                }
                case 4:
                {
                    return BadgeStyle.Light;
                }
                default:
                {
                    return BadgeStyle.Danger;
                }
            }
        }

  


        void OnEditSelectedRoles(object value)
        {
            var selectedRoles = ((IEnumerable<string>) value).ToList();
            EditingUser.Roles = selectedRoles;
        }

        void OnCreateSelectedRoles(object value)
        {
            var selectedRoles = ((IEnumerable<string>) value).ToList();
            NewUser.Roles = selectedRoles;
        }

        public Task HideEditingModal()
        {
            return EditingModal.Hide();
        }

        async Task ShowLoading()
        {
            IsLoading = true;

            await Task.Yield();

            IsLoading = false;
        }

        public async Task OnChangeFile(InputFileChangeEventArgs e)
        {
            EnclosedFile = e.GetMultipleFiles().FirstOrDefault();
        }

        public async Task ImportUserDataOfExcel()
        {
            if (EnclosedFile!= null)
            {
                TriggeredWithoutFile = false;
                await InvokeAsync(async () =>
                {
                    var fileDto = await _uploadService.UploadExcelFileOfUsers(EnclosedFile);
                    UserValidatorExcel = await _userManagerService.CreateUsersFromCSVFileAndDefineRoles(fileDto);
                    if (UserValidatorExcel.IsSuccessful)
                    {
                        HideImportExcelFileModal();
                        await GetUsers();
                    }
                    else
                    {
                        throw new FailedOperation("");
                    }

                }, ActionType.UploadFile, true);
            }
            else
            {
                TriggeredWithoutFile = true;
            }
        }

        public async Task DownloadExcelSampleFileOfUser()
        {
            string pathBase = Path.Combine(_webHostEnvironment.WebRootPath,@"excels\userExcelSample\sample.csv");
            var bytes = await FileHelper.GetBytesOfExcelFile(pathBase);
            await _downloadFile.DownloadFileAsync(bytes,"csv","sample");
        }

        public async Task DownloadInstructionFileOfCreatingUser()
        {
            string pathBase = Path.Combine(_webHostEnvironment.WebRootPath,@"excels\userExcelSample\instruction.csv");
            var bytes = await FileHelper.GetBytesOfExcelFile(pathBase);
            await _downloadFile.DownloadFileAsync(bytes,"csv","instruction");
        }
        
        void PickedColumnsChanged(DataGridPickedColumnsChangedEventArgs<UserWithNavigationPropertiesDto> args)
        {
        
        }
        async void ExportData()
        {
            
        }















    }
}