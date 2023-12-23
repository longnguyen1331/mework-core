using Blazorise;
using Contract;
using Contract.Posts;
using Core.Enum;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Radzen;

namespace WebClient.Pages.Admin.Posts
{
    public partial class Post
    {
        public List<PostDto> Posts = new List<PostDto>();
        public CreateUpdatePostDto NewPost = new CreateUpdatePostDto();
        public CreateUpdatePostDto EditingPost = new CreateUpdatePostDto();
        public List<BaseDto> PostCategories = new List<BaseDto>();
        public Guid EditPostId { get; set; }
        [Inject] IMessageService _messageService { get; set; }
        
        public Modal CreateModal;
        public Modal EditingModal;
        public string HeaderTitle = "Post Category";
        public IEnumerable<string> Claims = new List<string>();
        public IBrowserFile? NewImageFile { get; set; }
        public IBrowserFile? EditingImageFile { get; set; }

        public bool IsLoading { get; set; } = false;

        public Post()
        {
        }
        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    HeaderTitle = L["Post"];
                    await GetPosts();
                    StateHasChanged();
                }, ActionType.LoadData, false);
            }
        }

        public async Task GetPosts()
        {
            var result = await _postService.GetListAsync();
            if (result.IsSuccess)
                Posts = result.Data;
        }


        public async Task DeletePost(Guid id)
        {
            await InvokeAsync(async () =>
            {
                await _postService.DeleteAsync(id);
                await GetPosts();
            }, ActionType.Delete, true);
        }

        public async Task ShowConfirmMessage(Guid id)
        {
            if (await _messageService.Confirm(L["Confirmation.Message"], L["Confirmation"]))
            {
                await DeletePost(id);
            }
        }


        async Task GoToEditPage(string? id)
        {
            _navigationManager.NavigateTo($"posts/edit/{id}");
        }

        async Task GoToCreatePage()
        {
            _navigationManager.NavigateTo($"posts/create");
        }

        async Task ExportToExcel()
        {
            try
            {
                IsLoading = true;
                PostFitlerPagingDto filter = new PostFitlerPagingDto()
                {
                    Take = 0
                };
                var result = await _postService.ExportExcel(filter);

                if (result.IsSuccess)
                {
                    var fileName = $"post_{DateTime.Now.ToString("yyyy-MM-dd")}.xlsx";

                    await JS.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(result.Data));
                }
                else
                    NotifyMessage(NotificationSeverity.Error, result.Message, 2000);

                IsLoading = false;
            }
            catch
            {
                IsLoading = false;
            }
        }

        async Task ImportFromExcel(InputFileChangeEventArgs e)
        {
            try
            {
                IsLoading = true;

                var result = await _postService.CreatePostFromExcelFileAsync(e.File);

                if (result.IsSuccess)
                    NotifyMessage(NotificationSeverity.Success, "Import successful", 2000);
                else
                    NotifyMessage(NotificationSeverity.Error, result.Message, 5000);

                await GetPosts();

                IsLoading = false;
            }
            catch
            {
                IsLoading = false;
            }
        }
    }
}