using Blazorise;
using Contract.PostCategories;
using Core.Enum;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using WebClient.Exceptions;
using WebClient.Setting;

namespace WebClient.Pages.Admin
{
	public partial class PostCategory
    {
        public List<PostCategoryDto> PostCategories = new List<PostCategoryDto>();
        public CreateUpdatePostCategoryDto NewPostCategory = new CreateUpdatePostCategoryDto();
        public CreateUpdatePostCategoryDto EditingPostCategory = new CreateUpdatePostCategoryDto();
        public Guid EditPostCategoryId { get; set; }
        [Inject]  IMessageService _messageService { get; set; }

        public Modal CreateModal;
        public Modal EditingModal;
        public string HeaderTitle = "Post Category";
        public IEnumerable<string> Claims = new List<string>();
        public IBrowserFile? NewImageFile { get; set; }
        public IBrowserFile? EditingImageFile { get; set; }

        public string Base64EncodedImageData { set; get; }

        public PostCategory()
        {
        }
        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    HeaderTitle = L["PostCategory"];
                    await GetPostCategories();
                    StateHasChanged();
                }, ActionType.LoadData, false);
            }
        }

        public async Task GetPostCategories()
        {
            PostCategories = await _postCategoryService.GetListAsync();
        }

        public async Task CreatePostCategory()
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
                NewPostCategory.ImageId = fileIds.FirstOrDefault();

                await _postCategoryService.CreateAsync(input: NewPostCategory);
                HideNewModal();
                await GetPostCategories();
            }, ActionType.Create, true);
        }

        public async Task UpdatePostCategory()
        {
            await InvokeAsync(async () =>
            {
                if (EditingImageFile is null)
                {
                    if (EditingPostCategory.ImageId == null || EditingPostCategory.ImageId.Value == Guid.Empty)
                    {
                        throw new FailedOperation(L["AppConfig.ImportLogoAndIcon"]);
                    }
                }

                if (EditingImageFile is not null)
                {
                    var fileId = await _uploadService.UploadImage1(EditingImageFile);
                    EditingPostCategory.ImageId = fileId;
                }

                await _postCategoryService.UpdateAsync(EditingPostCategory, EditPostCategoryId);
                HideEditModal();
                await GetPostCategories();
            },ActionType.Update,true);
            
        }

        public async Task DeletePostCategory(Guid id)
        {
            await InvokeAsync(async () =>
            {
                await _postCategoryService.DeleteAsync(id);
                HideEditModal();
                await GetPostCategories();
            },ActionType.Delete,true);
            
        }

        public async Task ShowConfirmMessage(Guid id)
        {
            if (await _messageService.Confirm(L["Confirmation.Message"], L["Confirmation"]))
            {
                await DeletePostCategory(id);
            }
        }

        public void ShowNewModal()
        {
            NewPostCategory = new CreateUpdatePostCategoryDto();
            CreateModal.Show();
        }

        public void HideNewModal()
        {
            CreateModal.Hide();
        }

        public Task ShowEditingModal(PostCategoryDto postCategory)
        {
            EditingPostCategory = new CreateUpdatePostCategoryDto();
            EditingPostCategory = ObjectMapper.Map<PostCategoryDto, CreateUpdatePostCategoryDto>(postCategory);
            EditPostCategoryId = postCategory.Id;

            InvokeAsync(async () =>
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        using (var response = await client.GetAsync(postCategory.ImageUrl))
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
            });

            return EditingModal.Show();
        }

        public void HideEditModal()
        {
            EditingModal.Hide();
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
    }
}