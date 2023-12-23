using Blazorise;
using Contract;
using Contract.PostCategories;
using Contract.Posts;
using Contract.Services;
using Contract.ServiceTypes;
using Contract.WebMenus;
using Core.Enum;
using Core.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using WebClient.Exceptions;
using WebClient.Setting;

namespace WebClient.Pages.Admin
{
    public partial class WebMenu
    {
        public List<WebMenuDto> HierarchicalWebMenus = new List<WebMenuDto>();
        public List<WebMenuDto> WebMenus = new List<WebMenuDto>();

        public List<EnumBaseDto> MenuTypes = new List<EnumBaseDto>();
        public CreateUpdateWebMenuDto NewWebMenu = new CreateUpdateWebMenuDto();
        public CreateUpdateWebMenuDto EditingWebMenu = new CreateUpdateWebMenuDto();

        public List<PostDto> Posts = new List<PostDto>();
        public List<ServiceDto> Services = new List<ServiceDto>();
        public List<ServiceTypeDto> ServiceTypes = new List<ServiceTypeDto>();
        public List<PostCategoryDto> PostCategories = new List<PostCategoryDto>();

        public Guid? SelectedServiceType, SelectedPostCategory, SelectedPostDetail, SelectedServiceDetail;
        public Guid EditingWebMenuId { get; set; }
         [Inject]  IMessageService _messageService { get; set; }
        public string Base64EncodedImageData { set; get; }
        public IBrowserFile? NewImageFile { get; set; }
        public IBrowserFile? EditingImageFile { get; set; }
        public Modal CreateModal;
        public Modal EditingModal;
        public string HeaderTitle = "Web Menu";
        public Guid? ImageEditId;
        public int? SelectedMenuType;
        public string UrlRef { set; get; } = string.Empty;
        private bool HideUrlTextBox { get; set; } = false;
        private bool HidePostCategory { get; set; } = true;
        private bool HideServiceType { get; set; } = true;
        private bool HideServiceDetail { get; set; } = true;
        private bool HidePostDetail { get; set; } = true;
        public WebMenu()
        {
            
        }
        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    HeaderTitle = L["WebMenu"];
                    await GetWebMenus();
                    GetMenuType();
                    StateHasChanged();
                }, ActionType.LoadData, false);
            }
        }

        public void GetMenuType()
        {
            MenuTypes = ((WebMenuType[])Enum.GetValues(typeof(WebMenuType))).Select(c => new EnumBaseDto() { Value = (int)c, Text = c.ToString() }).ToList();
        }

        public async Task GetWebMenus()
        {
            var menuResponse = await _webMenuService.GetListAsync();
            if (menuResponse.IsSuccess)
            {
                HierarchicalWebMenus = menuResponse.Data;
                WebMenus = HierarchicalWebMenus.Clone();
                foreach (var item in HierarchicalWebMenus)
                {
                    var childWebMenus =
                        HierarchicalWebMenus.Where(x => x.ParentMenuId == item.Id).ToList();
                    item.ChildWebMenu = childWebMenus;
                }
                HierarchicalWebMenus = HierarchicalWebMenus.Where(x => x.ParentMenuId == null).ToList();
            }
        }

        public async Task GetPosts(string filterText = "", int skip = 0, int take = 10)
        {
            try
            {
                var result = await _postService.GetListPagingAsync(new BaseFilterPagingDto()
                {
                    FilterText = filterText,
                    Skip = skip,
                    Take = take
                });

                if (result.IsSuccess)
                {
                    Posts = result.Data;
                }
            }
            catch (Exception ex)
            {

            }
        }
        public async Task GetPostCategories()
        {
            PostCategories = await _postCategoryService.GetListAsync();
        }
      
        public async Task CreateWebMenu()
        {
            try
            {
                await InvokeAsync(async () =>
                {
                    if (NewImageFile is null)
                    {
                        throw new FailedOperation(L["AppConfig.ImportLogoAndIcon"]);
                    }

                    var files = new List<IBrowserFile>();
                    files.Add(NewImageFile);
                    var fileIds = await _uploadService.UploadImages(files);

                    NewWebMenu.ImageId = fileIds.FirstOrDefault();
                    NewWebMenu.MenuType = SelectedMenuType.Value;
                    NewWebMenu.UrlRef = SelectedMenuType.Value != (int)WebMenuType.Url ? UrlRef : NewWebMenu.UrlRef;
                    await _webMenuService.CreateAsync(input: NewWebMenu);
                    HideNewModal();
                    await GetWebMenus();
                }, ActionType.Create, true);
            }
            catch (Exception ex)
            {
                NotifyMessage(NotificationSeverity.Error, ex.Message, 4000);
            }
        }

        public async Task UpdateWebMenu()
        {
            await InvokeAsync(async () =>
            {
                if (ImageEditId != EditingWebMenu.ImageId)
                {
                    if (EditingImageFile is null)
                    {
                        if (EditingWebMenu.ImageId == null || EditingWebMenu.ImageId.Value == Guid.Empty)
                        {
                            throw new FailedOperation(L["AppConfig.ImportLogoAndIcon"]);
                        }
                    }

                    if (EditingImageFile is not null)
                    {
                        var fileId = await _uploadService.UploadImage1(EditingImageFile);
                        EditingWebMenu.ImageId = fileId;
                    }
                }

                EditingWebMenu.MenuType = SelectedMenuType.Value;
                EditingWebMenu.UrlRef = SelectedMenuType.Value != (int)WebMenuType.Url ? UrlRef : EditingWebMenu.UrlRef;
                await _webMenuService.UpdateAsync(EditingWebMenu, EditingWebMenuId);
                HideEditModal();    
                await GetWebMenus();
            },ActionType.Update,true);
            
        }

        public async Task DeleteWebMenu(Guid id)
        {
            await InvokeAsync(async () =>
            {
                await _webMenuService.DeleteAsync(id);
                HideEditModal();
                await GetWebMenus();
            },ActionType.Delete,true);
            
        }

        public async Task ShowConfirmMessage(object value)
        {
            var webMenu = (WebMenuDto)value;

            if (await _messageService.Confirm(L["Confirmation.Message"], L["Confirmation"]))
            {
                await DeleteWebMenu(webMenu.Id);
            }
        }

        public async Task ShowNewModal()
        {
            NewWebMenu = new CreateUpdateWebMenuDto();
            Base64EncodedImageData = null;
            NewImageFile = null;
            await InvokeAsync(async () =>
            {
                await OnChangeSelectWebMenu((int)WebMenuType.Url);
            });

            CreateModal.Show();
        }

        public void HideNewModal()
        {
            CreateModal.Hide();
        }

        public async Task ShowEditingModal(object value)
        {
            var webMenu = (WebMenuDto)value;
            ImageEditId = null;

            EditingWebMenu = new CreateUpdateWebMenuDto();
            EditingWebMenu = ObjectMapper.Map<WebMenuDto, CreateUpdateWebMenuDto>(webMenu);
            EditingWebMenuId = webMenu.Id;
            ImageEditId = webMenu.ImageId;

            await OnChangeSelectWebMenu(EditingWebMenu.MenuType);
            if (EditingWebMenu.MenuType == (int)WebMenuType.PostDetail)
            {
                SelectedPostDetail = !string.IsNullOrEmpty(EditingWebMenu.UrlRef) ? Guid.Parse(EditingWebMenu.UrlRef) : null;
                UrlRef = EditingWebMenu.UrlRef;
            }
         
            else if (EditingWebMenu.MenuType == (int)WebMenuType.PostCategory)
            {
                SelectedPostCategory = !string.IsNullOrEmpty(EditingWebMenu.UrlRef) ? Guid.Parse(EditingWebMenu.UrlRef) : null;
                UrlRef = EditingWebMenu.UrlRef;
            }
          

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (var response = await client.GetAsync(EditingWebMenu.ImageUrl))
                    {
                        byte[] imageBytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                        Base64EncodedImageData = Convert.ToBase64String(imageBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                Base64EncodedImageData = null;
            }

            EditingModal.Show();
        }

        public void HideEditModal()
        {
            EditingModal.Hide();
        }
        
        void OnChangeNewWebMenu(TreeEventArgs args)
        {
            var webMenu = (WebMenuDto) args.Value;
            NewWebMenu.ParentMenuId = webMenu.Id;
        }
       
        void OnChangeUpdateWebMenu(TreeEventArgs args)
        {
            
            var webMenu = (WebMenuDto) args.Value;
            
            if (EditingWebMenuId != webMenu.Id)
            {
                if (IsChildOfAssignmentWebMenu(EditingWebMenuId,webMenu))
                {
                    NotifyMessage(NotificationSeverity.Warning, "You can't choose its child",4000);
                }
                else
                {
                    EditingWebMenu.ParentMenuId = webMenu.Id;
                }
            }
        }

        bool IsChildOfAssignmentWebMenu(Guid assignmentId,WebMenuDto dto)
        {
            var childWebMenus = new List<WebMenuDto>();
            var webMenu = WebMenus.FirstOrDefault(x => x.Id == assignmentId);
            GetAllChildOfWebMenu(childWebMenus, webMenu);

            var item = childWebMenus.FirstOrDefault(x => x.Id == dto.Id);

            if (item != null) return true;

            return false;

        }       

        void GetAllChildOfWebMenu(List<WebMenuDto> childs,WebMenuDto dto)
        {
            var items = WebMenus.Where(x => x.ParentMenuId == dto.Id);
            childs.AddRange(items);
            foreach (var item in items)
            {
                GetAllChildOfWebMenu(childs, item);
            }
            
        }
        async Task OnChangeImageFileAtNewModal(InputFileChangeEventArgs e)
        {
            await InvokeAsync(async () =>
            {
                NewImageFile = null;
                NewImageFile = e.File;

                using (var ms = new MemoryStream())
                {
                    await e.File.OpenReadStream(25214400).CopyToAsync(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    var bytes = ms.ToArray();
                    Base64EncodedImageData = Convert.ToBase64String(bytes);
                }

                if (e.File.Size > BlazorSetting.Document_FILE_LENGTH_LIMIT)
                    throw new FailedOperation(@L["FileSoBig"]);
            }, ActionType.UploadFile, false);
        }
        async Task OnChangeImageFileAtEditingModal(InputFileChangeEventArgs e)
        {
            await InvokeAsync(async () =>
            {
                EditingImageFile = null;
                EditingImageFile = e.File;

                using (var ms = new MemoryStream())
                {
                    await e.File.OpenReadStream(25214400).CopyToAsync(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    var bytes = ms.ToArray();
                    Base64EncodedImageData = Convert.ToBase64String(bytes);
                }

                if (e.File.Size > BlazorSetting.Document_FILE_LENGTH_LIMIT)
                    throw new FailedOperation(@L["FileSoBig"]);
            }, ActionType.UploadFile, false);
        }
        public async Task OnChangeSelectWebMenu(object value)
        {
            SelectedMenuType = (int)value;
            UrlRef = string.Empty;
            switch ((int)value)
            {
                case (int)WebMenuType.Url:
                    HideUrlTextBox = false;
                    HidePostCategory = true;
                    HidePostDetail = true;
                    HideServiceType = true;
                    HideServiceDetail = true;

                    NewWebMenu.MenuSuffix = EditingWebMenu.MenuSuffix = "";

                    break;
                case (int)WebMenuType.ServiceType:
                    HideServiceType = false;
                    await GetServiceType();
                    //Getservice

                    HideUrlTextBox = true;
                    HidePostCategory = true;
                    HidePostDetail = true;
                    HideServiceDetail = true;
                    NewWebMenu.MenuSuffix = EditingWebMenu.MenuSuffix = WebMenuType.ServiceType.GetDescriptionOrName();
                    break;

                case (int)WebMenuType.ServiceDetail:
                    HideServiceDetail = false;
                    await GetServices();
                    //get service detail

                    HideUrlTextBox = true;
                    HidePostCategory = true;
                    HideServiceType = true;
                    HidePostDetail = true;
                    NewWebMenu.MenuSuffix = EditingWebMenu.MenuSuffix = WebMenuType.ServiceDetail.GetDescriptionOrName();
                    break;

                case (int)WebMenuType.PostCategory:
                    HidePostCategory = false;
                    await GetPostCategories();
                    //get posts

                    HideUrlTextBox = true;
                    HideServiceType = true;
                    HidePostDetail = true;
                    HideServiceDetail = true;
                    NewWebMenu.MenuSuffix = EditingWebMenu.MenuSuffix = WebMenuType.PostCategory.GetDescriptionOrName();
                    break;

                case (int)WebMenuType.PostDetail:
                    HidePostDetail = false;
                    //get post detail
                    await GetPosts();
                    HideUrlTextBox = true;
                    HideServiceType = true;
                    HideServiceDetail = true;
                    HidePostCategory = true;
                    NewWebMenu.MenuSuffix = EditingWebMenu.MenuSuffix = WebMenuType.PostDetail.GetDescriptionOrName();
                    break;
                default: break;
            }
        }
        void ServiceOnLoadData(string value)
        {
            InvokeAsync(async () => {
                await GetServices(value);
                StateHasChanged();
            });
        }

        public async Task GetServiceType()
        {
            ServiceTypes = await _serviceTypeService.GetListAsync();
        }

        public async Task GetServices(string filterText = "", int skip = 0, int take = 10)
        {
            try
            {
                var result = await _serviceService.GetListPagingAsync(new BaseFilterPagingDto()
                {
                    FilterText = filterText,
                    Skip = skip,
                    Take = take
                });
                if (result.IsSuccess)
                {
                    Services = result.Data;
                }
                else
                {
                    NotifyMessage(NotificationSeverity.Error, result.Message, 4000);
                }
            }
            catch (Exception ex)
            {
                NotifyMessage(NotificationSeverity.Error, ex.Message, 4000);
            }
        }
        public async Task OnChangeSelect(object value)
        {
            if (value != null)
            {
                UrlRef = ((Guid)value).ToString();
            }
        }

        void PostOnLoadData(string value)
        {
            InvokeAsync(async () => {
                await GetPosts(value);
                StateHasChanged();
            });
        }

      

        void PostCategoryOnLoadData(string value)
        {
            InvokeAsync(async () => {
                await GetPostCategories();
                StateHasChanged();
            });
        }

    }
}