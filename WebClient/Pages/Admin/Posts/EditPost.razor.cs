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
    public partial class EditPost
    {
        public PostDto Post = new PostDto();
        public CreateUpdatePostDto EditingPost = new CreateUpdatePostDto();
        public List<BaseDto> PostCategories = new List<BaseDto>();
        public Guid? EditPostId { get; set; }
        public Guid? ImageEditId;
        public string HeaderTitle = "Edit Post";
        public IEnumerable<string> Claims = new List<string>();
        public IBrowserFile? EditingImageFile { get; set; }

        public string Base64EncodedImageData { set; get; }
        [Parameter]
        public string Id { get; set; }
        public List<string> Tags = new List<string>();

        public EditPost()
        {
        }
        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    HeaderTitle = L["EditPost"];
                    await GetPostCategories();
                    await BuildPostDto();
                    StateHasChanged();
                }, ActionType.LoadData, false);
            }
        }

        public async Task BuildPostDto()
        {
            EditingPost = new CreateUpdatePostDto();
            EditPostId = null;
            if (!string.IsNullOrEmpty(Id))
            {
                HeaderTitle = L["EditPost"];
                var result = await _postService.GetByIdAsync(Guid.Parse(Id));
                if (result.IsSuccess)
                {
                    Post = result.Data;
                    ImageEditId = null;
                    EditingPost = ObjectMapper.Map<PostDto, CreateUpdatePostDto>(Post);
                    EditPostId = Post.Id;
                    ImageEditId = EditingPost.PictureId;

                    if (!string.IsNullOrEmpty(EditingPost.Tags))
                    {
                        Tags = EditingPost.Tags.Split(',').ToList();
                        EditingPost.Tags = "";
                    }
                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            using (var response = await client.GetAsync(Post.PictureUrl))
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
                }
                else
                {
                    NotifyMessage(NotificationSeverity.Error, result.Message, 4000);
                }
            }
        }

        public async Task GetPostCategories()
        {
            var postCategories = await _poseCategoryService.GetListAsync();

            PostCategories = ObjectMapper.Map<List<PostCategoryDto>, List<BaseDto>>(postCategories);
        }
        public async Task OnEditPost()
        {
            if (EditingImageFile is null)
            {
                if (EditingPost.PictureId == null || EditingPost.PictureId.Value == Guid.Empty)
                {
                    throw new FailedOperation(L["AppConfig.ImportLogoAndIcon"]);
                }
            }


            EditingPost.Tags = string.Join(",", Tags);

            if (EditingImageFile is not null)
            {
                var fileId = await _uploadService.UploadImage1(EditingImageFile);
                EditingPost.PictureId = fileId;
            }

            var result = await _postService.UpdateAsync(EditingPost, EditPostId.Value);
            if (result.IsSuccess)
            {
                NotifyMessage(NotificationSeverity.Success, "Edit Success", 4000);
            }
            else
            {
                NotifyMessage(NotificationSeverity.Error, result.Message, 4000);
            }
        }

        async Task OnChangeImageFile(InputFileChangeEventArgs e)
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

        async Task BackToList()
        {
            _navigationManager.NavigateTo($"posts");
        }

        public async Task TagOnChange(string value)
        {
            if (!Tags.Any(x => x.Trim().ToLower().Equals(value.Trim().ToLower())))
                Tags.Add(value);
            EditingPost.Tags = "";
            await JSRuntime.InvokeVoidAsync("eval", $@"document.getElementsByName(""txtarea_tag"")[0].focus()");
        }

        public void RemoveTag(string value)
        {
            Tags.Remove(value);
        }

    }
}