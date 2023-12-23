using Contract;
using Contract.AppHistories;
using Core.Const;
using Core.Exceptions;
using Domain.AppHistories;
using Microsoft.EntityFrameworkCore;
using SqlServ4r.Repository.AppHistorys;
using SqlServ4r.Repository.Users;
using System.Net;
using Volo.Abp.DependencyInjection;

namespace Application.AppHistorys
{
    public class AppHistoryService : ServiceBase,IAppHistoryService,ITransientDependency
    {
       private readonly AppHistoryRepository _appHistoryRepository;
       private readonly UserRepository _userRepository;


        public AppHistoryService(AppHistoryRepository appHistoryRepository, UserRepository userRepository)
        {
            _appHistoryRepository = appHistoryRepository;
            _userRepository = userRepository;
        }


        public async Task CreateAsync(CreateUpdateAppHistoryDto input)
        {
            var appHistory = ObjectMapper.Map<CreateUpdateAppHistoryDto, AppHistory>(input);
            //appHistory.User = await _userRepository.FirstOrDefaultAsync(x=>x.Id == appHistory.UserId); 
            await _appHistoryRepository.AddAsync(appHistory);
            //return ObjectMapper.Map<AppHistory,AppHistoryDto>(appHistory);
        }

        public async Task<AppHistoryDto> UpdateAsync(CreateUpdateAppHistoryDto input, Guid id)
        {
            var item = await _appHistoryRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (item is null)
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }
            
            var appHistory = ObjectMapper.Map(input,item);
            await _appHistoryRepository.UpdateAsync(appHistory);
            return ObjectMapper.Map<AppHistory,AppHistoryDto>(appHistory);
        }

        public async Task<ApiResponseBase<List<AppHistoryDto>>> GetListAsync()
        {
            ApiResponseBase<List<AppHistoryDto>> result = new ApiResponseBase<List<AppHistoryDto>>();

            try
            {
                var appHistorys = await _appHistoryRepository.GetQueryable().
                   OrderByDescending(x => x.Date).ToListAsync();
                result.Data = ObjectMapper.Map<List<AppHistory>, List<AppHistoryDto>>(appHistorys);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ApiResponseBase<AppHistorySearchResponseDto>> GetListAsync(AppHistoryFilterPagingDto filter)
        {
            ApiResponseBase<AppHistorySearchResponseDto> result = new ApiResponseBase<AppHistorySearchResponseDto>();

            try
            {
                result.Data = new AppHistorySearchResponseDto();

                var appHistorys = _appHistoryRepository
                    .GetQueryable()
                    .Include(x => x.User)
                    .AsQueryable()
                    .Where(x => (filter.Date != null ? x.Date.Date == filter.Date : true)
                    && (!string.IsNullOrEmpty(filter.Functions) ? x.Functions.Contains(filter.Functions.Trim()) : true)
                    && (!string.IsNullOrEmpty(filter.IpAddress) ? x.IpAddress.Contains(filter.IpAddress.Trim()) : true)
                    && (!string.IsNullOrEmpty(filter.Operation) ? x.Operation.Contains(filter.Operation.Trim()) : true)
                    && (!string.IsNullOrEmpty(filter.FullName) ? (x.User.FirstName + " " + x.User.LastName).Contains(filter.FullName.Trim()) : true));

                result.Data.TotalItem = appHistorys.Count();

                if (filter.Take > 0)
                    appHistorys = appHistorys.OrderByDescending(x => x.Date).Skip(filter.Skip).Take(filter.Take);

                result.Data.Result = ObjectMapper.Map<List<AppHistory>, List<AppHistoryDto>>(await appHistorys.ToListAsync());
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}