using Blazorise;
using Contract;
using Contract.Posts;
using Contract.WebBanners;
using Core.Enum;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using WebClient.Exceptions;
using WebClient.Setting;

namespace WebClient.Pages.Admin
{
    public partial class WebBanner
    {
        public List<WebBannerDto> WebBanners = new List<WebBannerDto>();
        public CreateUpdateWebBannerDto NewWebBanner = new CreateUpdateWebBannerDto();
        public CreateUpdateWebBannerDto EditingWebBanner = new CreateUpdateWebBannerDto();
        public Guid EditWebBannerId { get; set; }
        [Inject]  IMessageService _messageService { get; set; }

        public Guid? SelectedWebBannerTypeId;
        public Guid? WebsiteBannerEditId;
        public Guid? MobileBannerEditId;

        public Modal CreateModal;
        public Modal EditingModal;
        public string HeaderTitle = "Web Banner";
        public IEnumerable<string> Claims = new List<string>();
        public List<EnumBaseDto> BannerTypes = new List<EnumBaseDto>();
        public List<PostDto> Posts = new List<PostDto>();

        public int? SelectedBannerType;
        public Guid?  SelectedPost;
        public IBrowserFile? WebsiteBannerFile { get; set; }
        public IBrowserFile? MobileBannerFile { get; set; }
        public string Base64EncodedWebsiteBannerData { set; get; }
        public string Base64EncodedMobileBannerData { set; get; }
        private bool HideUrlTextBox { get; set; } = false;
        private bool HideService { get; set; } = true;
        private bool HidePost { get; set; } = true;
        public string UrlRef { set; get; } = string.Empty;

        public WebBanner()
        {
        }
        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    HeaderTitle = L["WebBanner"];
                    await GetWebBanners();
                    GetBannerType();
                    StateHasChanged();
                }, ActionType.LoadData, false);
            }
        }

        public async Task GetWebBanners()
        {
            try
            {
                var data = await _webBannerService.GetListAsync();
                WebBanners = data;
            }
            catch (Exception ex) 
            { 
            
            }
        }
        public async Task GetPosts(string filterText = "", int skip = 0, int take = 10)
        {
            try
            {
                var result  = await _postService.GetListPagingAsync(new BaseFilterPagingDto()
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
        public void GetBannerType()
        {
            BannerTypes = ((WebBannerType[])Enum.GetValues(typeof(WebBannerType))).Select(c => new EnumBaseDto() { Value = (int)c, Text = c.ToString() }).ToList();
        }
        public async Task CreateWebBanner()
        {
            await InvokeAsync(async () =>
            {
                NewWebBanner.WebsiteBannerId = WebsiteBannerEditId;
                NewWebBanner.MobileBannerId = MobileBannerEditId;
                NewWebBanner.BannerType = SelectedBannerType.Value;
                NewWebBanner.UrlRef = SelectedBannerType.Value != (int)WebBannerType.Url ? UrlRef : NewWebBanner.UrlRef;
                await _webBannerService.CreateAsync(input: NewWebBanner);
                HideNewModal();
                await GetWebBanners();
                await OnChangeSelectWebBanner(NewWebBanner.BannerType);
            }, ActionType.Create, true);
        }

        public async Task UpdateWebBanner()
        {
            await InvokeAsync(async () =>
            {
                if(WebsiteBannerEditId != EditingWebBanner.WebsiteBannerId)
                {
                    if (WebsiteBannerFile is null)
                    {
                        if (EditingWebBanner.WebsiteBannerId == null || EditingWebBanner.WebsiteBannerId.Value == Guid.Empty)
                        {
                            throw new FailedOperation(L["AppConfig.ImportLogoAndIcon"]);
                        }
                    }

                    if (WebsiteBannerFile is not null)
                    {
                        var fileId = await _uploadService.UploadImage1(WebsiteBannerFile);
                        EditingWebBanner.WebsiteBannerId = fileId;
                    }
                }

                if (MobileBannerEditId != EditingWebBanner.MobileBannerId)
                {
                    if (MobileBannerFile is null)
                    {
                        if (EditingWebBanner.MobileBannerId == null || EditingWebBanner.MobileBannerId.Value == Guid.Empty)
                        {
                            throw new FailedOperation(L["AppConfig.ImportLogoAndIcon"]);
                        }
                    }

                    if (MobileBannerFile is not null)
                    {
                        var fileId = await _uploadService.UploadImage1(MobileBannerFile);
                        EditingWebBanner.MobileBannerId = fileId;
                    }
                }

                EditingWebBanner.BannerType = SelectedBannerType.Value;
                EditingWebBanner.UrlRef = SelectedBannerType != (int)WebBannerType.Url ? UrlRef : EditingWebBanner.UrlRef;

                await _webBannerService.UpdateAsync(EditingWebBanner, EditWebBannerId);
                HideEditModal();
                await GetWebBanners();
            },ActionType.Update,true);
            
        }

        public async Task DeleteWebBanner(Guid id)
        {
            await InvokeAsync(async () =>
            {
                await _webBannerService.DeleteAsync(id);
                HideEditModal();
                await GetWebBanners();
            },ActionType.Delete,true);
            
        }

        public async Task ShowConfirmMessage(Guid id)
        {
            if (await _messageService.Confirm(L["Confirmation.Message"], L["Confirmation"]))
            {
                await DeleteWebBanner(id);
            }
        }

        public void ShowNewModal()
        {
            NewWebBanner = new CreateUpdateWebBannerDto();
            SelectedBannerType = (int)WebBannerType.Url;
            CreateModal.Show();
        }

        public void HideNewModal()
        {
            CreateModal.Hide();
        }

        public async Task ShowEditingModal(WebBannerDto service)
        {
            WebsiteBannerEditId = null;
            MobileBannerEditId = null;
            Base64EncodedMobileBannerData = null;
            Base64EncodedWebsiteBannerData = null;
            EditingWebBanner = new CreateUpdateWebBannerDto();
            EditingWebBanner = ObjectMapper.Map<WebBannerDto, CreateUpdateWebBannerDto>(service);
            EditWebBannerId = service.Id;

            await OnChangeSelectWebBanner(EditingWebBanner.BannerType);
            if (EditingWebBanner.BannerType == (int)WebBannerType.News)
            {
                SelectedPost = !string.IsNullOrEmpty(EditingWebBanner.UrlRef) ? Guid.Parse(EditingWebBanner.UrlRef) : null;
            }
          
            if (!string.IsNullOrEmpty(service.WebsiteBannerUrl))
            {
                using (HttpClient client = new HttpClient())
                {
                    using (var response = await client.GetAsync(service.WebsiteBannerUrl))
                    {
                        byte[] imageBytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                        Base64EncodedWebsiteBannerData = Convert.ToBase64String(imageBytes);
                    }
                }
            }

            if (!string.IsNullOrEmpty(service.MobileBannerUrl))
            {
                using (HttpClient client = new HttpClient())
                {
                 
                    using (var response = await client.GetAsync(service.MobileBannerUrl))
                    {
                        byte[] imageBytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                        Base64EncodedMobileBannerData = Convert.ToBase64String(imageBytes);
                    }
                }
            }

            EditingModal.Show();
        }

        public void HideEditModal()
        {
            EditingModal.Hide();
        }
        async Task OnChangeWebsiteFileAtModal(InputFileChangeEventArgs e)
        {
            await InvokeAsync(async () =>
            {
                WebsiteBannerFile = null;
                WebsiteBannerFile = e.File;

                using (var ms = new MemoryStream())
                {
                    await e.File.OpenReadStream(25214400).CopyToAsync(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    var bytes = ms.ToArray();
                    Base64EncodedWebsiteBannerData = Convert.ToBase64String(bytes);
                }

                var files = new List<IBrowserFile>();
                files.Add(WebsiteBannerFile);
                var fileIds = await _uploadService.UploadImages(files);
                WebsiteBannerEditId = fileIds.FirstOrDefault();

                if (e.File.Size > BlazorSetting.Document_FILE_LENGTH_LIMIT)
                    throw new FailedOperation(@L["FileSoBig"]);
            }, ActionType.UploadFile, false);
        }
        async Task OnChangeMobileFileAtModal(InputFileChangeEventArgs e)
        {
            await InvokeAsync(async () =>
            {
                MobileBannerFile = null;
                MobileBannerFile = e.File;

                using (var ms = new MemoryStream())
                {
                    await e.File.OpenReadStream(25214400).CopyToAsync(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    var bytes = ms.ToArray();
                    Base64EncodedMobileBannerData = Convert.ToBase64String(bytes);
                }

                var files = new List<IBrowserFile>();
                files.Add(MobileBannerFile);
                var fileIds = await _uploadService.UploadImages(files);
                MobileBannerEditId = fileIds.FirstOrDefault();

                if (e.File.Size > BlazorSetting.Document_FILE_LENGTH_LIMIT)
                    throw new FailedOperation(@L["FileSoBig"]);
            }, ActionType.UploadFile, false);
        }

        public async Task OnChangeSelectWebBanner(object value)
        {
            SelectedBannerType = (int)value;
            UrlRef = string.Empty;
            switch ((int)value)
            {
                case (int)WebBannerType.Url:
                    HideUrlTextBox = false;
                    HidePost = true;
                    HideService = true;
                    break;
              
                case (int)WebBannerType.News:
                    await GetPosts();
                    HideUrlTextBox = true;
                    HideService = true;
                    HidePost = false;
                    break;
                default: break; 
            }
        }

        void  PostOnLoadData(string value)
        {
            InvokeAsync(async () => {
                await GetPosts(value);
                StateHasChanged();
            });
        }

     

        public async Task OnChangeSelect(object value)
        {
            if(value != null)
            {
                UrlRef = ((Guid)value).ToString();
            }
        }
    }
}