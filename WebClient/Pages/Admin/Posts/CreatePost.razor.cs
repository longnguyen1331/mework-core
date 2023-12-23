using Blazorise;
using Contract;
using Contract.PostCategories;
using Contract.Posts;
using Core.Enum;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Radzen;
using WebClient.Exceptions;
using WebClient.Setting;

namespace WebClient.Pages.Admin.Posts
{
    public partial class CreatePost
    {
        public CreateUpdatePostDto CreatingPost = new CreateUpdatePostDto();
        public List<BaseDto> PostCategories = new List<BaseDto>();
        public Guid EditPostId { get; set; }
        [Inject] IMessageService _messageService { get; set; }

        public string HeaderTitle = "Create posts";
        public IEnumerable<string> Claims = new List<string>();
        public IBrowserFile? NewImageFile { get; set; }

        public string Base64EncodedImageData { set; get; }
        public List<string> Tags = new List<string>();

        public CreatePost()
        {
        }
        protected override async void 
            OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    HeaderTitle = L["CreatePost"];
                    await GetPostCategories();
                    StateHasChanged();
                }, ActionType.LoadData, false);
            }
        }

      
        public async Task GetPostCategories()
        {
            var postCategories = await _poseCategoryService.GetListAsync();

            PostCategories = ObjectMapper.Map<List<PostCategoryDto>, List<BaseDto>>(postCategories);
        }

        public async Task OnCreatePost()
        {
            try
            {
                if (NewImageFile is null)
                {
                    throw new FailedOperation(L["AppConfig.ImportLogoAndIcon"]);
                }

                var files = new List<IBrowserFile>();
                files.Add(NewImageFile);
                var fileIds = await _uploadService.UploadImages(files);
                CreatingPost.PictureId = fileIds.FirstOrDefault();

                var result = await _postService.CreateAsync(input: CreatingPost);
                if (result.IsSuccess)
                {
                    NotifyMessage(NotificationSeverity.Success, "Create Success", 4000);
                    await BackToList();
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

       
        async Task OnChangeImageFile(InputFileChangeEventArgs e)
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


        async Task BackToList()
        {
            _navigationManager.NavigateTo($"posts");
        }

        public async Task TagOnChange(string value)
        {
            if (!Tags.Any(x => x.Trim().ToLower().Equals(value.Trim().ToLower())))
                Tags.Add(value);
            CreatingPost.Tags = "";
            await JSRuntime.InvokeVoidAsync("eval", $@"document.getElementsByName(""txtarea_tag"")[0].focus()");
        }

        public void RemoveTag(string value)
        {
            Tags.Remove(value);
        }

    }
}