using Application.Backups;
using Contract;
using Contract.Backups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/backups/")]
    [Authorize]
    public class BackupController 
    {
        private BackupService _backupService;
        public BackupController(BackupService backupService)
        {
            _backupService = backupService;
        }
        
        [HttpPost]
        public async Task<ApiResponseBase<bool>> CreateAsync(CreateUpdateBackupDto input)
        {
            return await _backupService.CreateAsync(input); 
        }

        [HttpGet]
        [Route("TestConnection/{id}")]
        public async Task<ApiResponseBase<bool>> TestConnectionAsync(Guid id)
        {
            return await _backupService.TestConnectionAsync(id);
        }


        [HttpDelete]
        public async Task<ApiResponseBase<bool>> DeleteAsync(Guid id)
        {
            return await _backupService.DeleteAsync(id);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ApiResponseBase<BackupDto>> UpdateAsync(CreateUpdateBackupDto input, Guid id)
        {
            return  await _backupService.UpdateAsync(input,id);
        }

        [HttpPost]
        [Route("{search}")]
        public async Task<ApiResponseBase<BackupSearchResponseDto>> GetListAsync(BackupFilterPagingDto filter)
        {
            return await _backupService.GetListAsync(filter);
        }
    }
}