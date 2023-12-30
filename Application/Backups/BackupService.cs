using Contract;
using Contract.Backups;
using Core.Const;
using Core.Exceptions;
using Domain.Backups;
using Microsoft.EntityFrameworkCore;
using SqlServ4r.Repository.Backups;
using System.Net;
using Volo.Abp.DependencyInjection;

namespace Application.Backups
{
    public class BackupService : ServiceBase,IBackupService,ITransientDependency
    {
       private readonly BackupRepository _backupRepository;


        public BackupService(BackupRepository backupRepository)
        {
            _backupRepository = backupRepository;
        }


        public async Task<ApiResponseBase<bool>> CreateAsync(CreateUpdateBackupDto input)
        {
            ApiResponseBase<bool> result = new ApiResponseBase<bool>() { Data = true };
            try
            {
                var backup = ObjectMapper.Map<CreateUpdateBackupDto, Backup>(input);
                await _backupRepository.AddAsync(backup);
            }
            catch (Exception ex)
            {
                result.Data = false;
                result.Message = ex.Message;    
            }
            return result;
        }

        public async Task<ApiResponseBase<BackupDto>> UpdateAsync(CreateUpdateBackupDto input, Guid id)
        {
            ApiResponseBase<BackupDto> result = new ApiResponseBase<BackupDto>() { Data = null };
            try
            {
                var item = await _backupRepository.FirstOrDefaultAsync(x => x.Id == id);

                if (item is null)
                {
                    throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
                }

                var backup = ObjectMapper.Map(input, item);
                await _backupRepository.UpdateAsync(backup);
                result.Data = ObjectMapper.Map<Backup,BackupDto>(backup);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Message = ex.Message;
            }

            return result;
        }

      
        public async Task<ApiResponseBase<BackupSearchResponseDto>> GetListAsync(BackupFilterPagingDto filter)
        {
            ApiResponseBase<BackupSearchResponseDto> result = new ApiResponseBase<BackupSearchResponseDto>();

            try
            {
                result.Data = new BackupSearchResponseDto();

                var backups = _backupRepository
                    .GetQueryable()
                    .Include(x => x.User)
                    .AsQueryable()
                    .Where(x => (filter.FromDate.HasValue ? x.CreatedDate >= filter.FromDate.Value : true)
                    && (filter.ToDate.HasValue ? x.CreatedDate <= filter.ToDate.Value : true));

                result.Data.TotalItem = backups.Count();

                if (filter.Take > 0)
                    backups = backups.OrderByDescending(x => x.CreatedDate).Skip(filter.Skip).Take(filter.Take);

                result.Data.Result = ObjectMapper.Map<List<Backup>, List<BackupDto>>(await backups.ToListAsync());
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ApiResponseBase<bool>> DeleteAsync(Guid id)
        {

            ApiResponseBase<bool> result = new ApiResponseBase<bool>() { Data = true };
            try
            {

                var item = await _backupRepository.FirstOrDefaultAsync(x => x.Id == id);

                if (item is null)
                {
                    throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
                }

                _backupRepository.Remove(item);
            }
            catch (Exception ex)
            {
                result.Data = false;
                result.Message = ex.Message;
            }
            return result;

        }
    }
}