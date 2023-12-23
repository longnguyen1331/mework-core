using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise;
using Contract.Positions;
using Core.Enum;
using Microsoft.AspNetCore.Components;
using WebClient.LanguageResources;

namespace WebClient.Pages.Admin
{
    public partial class Position
    {
        public List<PositionDto> Positions = new List<PositionDto>();
        public CreateUpdatePositionDto NewPosition = new CreateUpdatePositionDto();
        public CreateUpdatePositionDto EditingPosition = new CreateUpdatePositionDto();
        public Guid EditPositionId { get; set; }
         [Inject]  IMessageService _messageService { get; set; }


        public Modal CreateModal;
        public Modal EditingModal;
        public string HeaderTitle = "Position";
        public IEnumerable<string> Claims = new List<string>();

        public Position()
        {
        }
        
       

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(async () =>
                {
                    HeaderTitle = L["Position"];
                    await GetPositions();
                    StateHasChanged();
                }, ActionType.LoadData, false);
            }
        }

        public async Task GetPositions()
        {
            Positions = await _positionService.GetListAsync();
        }

        public async Task CreatePosition()
        {
            await InvokeAsync(async () =>
            {
                await _positionService.CreateAsync(input: NewPosition);
                HideNewModal();
                await GetPositions();
            }, ActionType.Create, true);
        }

        public async Task UpdatePosition()
        {
            await InvokeAsync(async () =>
            {
                await _positionService.UpdateAsync(EditingPosition, EditPositionId);
                HideEditModal();
                await GetPositions();
            },ActionType.Update,true);
            
        }

        public async Task DeletePosition(Guid id)
        {
            await InvokeAsync(async () =>
            {
                await _positionService.DeleteAsync(id);
                HideEditModal();
                await GetPositions();
            },ActionType.Delete,true);
            
        }

        public async Task ShowConfirmMessage(Guid id)
        {
            if (await _messageService.Confirm(L["Confirmation.Message"], L["Confirmation"]))
            {
                await DeletePosition(id);
            }
        }

        public void ShowNewModal()
        {
            NewPosition = new CreateUpdatePositionDto();
            CreateModal.Show();
        }

        public void HideNewModal()
        {
            CreateModal.Hide();
        }


        public Task ShowEditingModal(PositionDto position)
        {
            EditingPosition = new CreateUpdatePositionDto();
            EditingPosition = ObjectMapper.Map<PositionDto, CreateUpdatePositionDto>(position);
            EditPositionId = position.Id;
            return EditingModal.Show();
        }

        public void HideEditModal()
        {
            EditingModal.Hide();
        }
        
        
      
    }
}