using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise;
using Contract.Backups;
using Core.Enum;
using Microsoft.AspNetCore.Components;
using Radzen;
using WebClient.LanguageResources;

namespace WebClient.Pages.Admin
{
    public partial class Backup
    {
        public List<BackupDto> Backups = new List<BackupDto>();
        public CreateUpdateBackupDto NewBackup = new CreateUpdateBackupDto();
        [Inject] IMessageService _messageService { get; set; }

        public Modal CreateUpdateModal;
        public string HeaderTitle = "Backup";
        public IEnumerable<string> Claims = new List<string>();
        public int TotalItem { set; get; } = 0;
        public int DefaultPageSize { set; get; } = 20;
        private BackupFilterPagingDto Filter = new BackupFilterPagingDto();
        public bool IsLoading { get; set; } = true;
        public Backup()
        {
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                IsLoading = true;
                await InvokeAsync(async () =>
                {
                    HeaderTitle = L["Backup"];
                    await GetBackups();
                    StateHasChanged();
                }, ActionType.LoadData, false);
                IsLoading = false;
            }
        }

        public async Task GetBackups(LoadDataArgs? args = null)
        {
            IsLoading = true;

            if (args != null)
            {
                Filter.Skip = args.Skip ?? 0;
                Filter.Take = args.Top ?? DefaultPageSize;

                if (args.Filters != null)
                {
                  
                }
            }

            var response = await _backupService.GetListAsync(Filter);
            if (response.IsSuccess)
            {
                Backups = response.Data.Result;
                TotalItem = response.Data.TotalItem;
            }
            else
                NotifyMessage(NotificationSeverity.Error, response.Message, 2000);

            IsLoading = false;

        }

        public async Task CreateOrUpdateBackup()
        {
            if (NewBackup.Id.HasValue)
            {

                await InvokeAsync(async () =>
                {
                    await _backupService.UpdateAsync(input: NewBackup, NewBackup.Id.Value);
                    HideModal();
                    await GetBackups();
                }, ActionType.Update, true);
            }
            else
            {
                await InvokeAsync(async () =>
                {
                    await _backupService.CreateAsync(input: NewBackup);
                    HideModal();
                    await GetBackups();
                }, ActionType.Create, true);
            }
        }

        public async Task DeleteBackup(Guid id)
        {
            await InvokeAsync(async () =>
            {
                await _backupService.DeleteAsync(id);
                HideModal();
                await GetBackups();
            }, ActionType.Delete, true);

        }

        public async Task ShowConfirmMessage(Guid id)
        {
            if (await _messageService.Confirm(L["Confirmation.Message"], L["Confirmation"]))
            {
                await DeleteBackup(id);
            }
        }

        public void ShowNewModal()
        {
            NewBackup = new CreateUpdateBackupDto();
            CreateUpdateModal.Show();
        }
        public async Task ShowEditingModal(BackupDto data)
        {
            NewBackup = new CreateUpdateBackupDto
            {
                Code = data.Code,
                Name = data.Name,
                Server = data.Server,   
                Id = data.Id,
                UserName = data.UserName,
                UserId = data.UserId,
                Password = data.Password,
                DbName = data.DbName,   
                FullFilePath = data.FullFilePath,   
            };
            CreateUpdateModal.Show();
        }

        public void HideModal()
        {
            CreateUpdateModal.Hide();
        }
    }
}