using Application.BackupDetails;
using Contract;
using Contract.BackupDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/backupDetails/")]
    [Authorize]
    public class BackupDetailController 
    {
        private BackupDetailService _backupDetailService;
        public BackupDetailController(BackupDetailService backupDetailService)
        {
            _backupDetailService = backupDetailService;
        }
        
        [HttpPost]
        public async Task<ApiResponseBase<bool>> CreateAsync(CreateUpdateBackupDetailDto input)
        {
            return await _backupDetailService.CreateAsync(input); 
        }

       

        [HttpPost]
        [Route("{search}")]
        public async Task<ApiResponseBase<BackupDetailsearchResponseDto>> GetListAsync(BackupDetailFilterPagingDto filter)
        {
            return await _backupDetailService.GetListAsync(filter);
        }
    }
}