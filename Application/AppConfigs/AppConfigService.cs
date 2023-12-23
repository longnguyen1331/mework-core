using Application.Helpers;
using Contract;
using Contract.AppConfigs;
using Core.Const;
using Core.Exceptions;
using Domain.AppConfigs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SqlServ4r.Repository.AppConfigs;
using SqlServ4r.Repository.Files;
using System.Net;
using Volo.Abp.DependencyInjection;

namespace Application.AppConfigs
{
    public class AppConfigService :ServiceBase, IAppConfigService,ITransientDependency
    {
        private readonly AppConfigRepository _appConfigRepository;
        private readonly StaticFileRepository _staticFileRepository;
        private readonly IConfiguration _configuration;
        
        public AppConfigService(AppConfigRepository appConfigRepository,
            StaticFileRepository staticFileRepository
                ,IConfiguration configuration
           )
        {
            _appConfigRepository = appConfigRepository;
            _configuration = configuration;
            _staticFileRepository = staticFileRepository;
        }


        public async Task<AppConfigDto> GetAppliedConfigAsync()
        {
            var config = await _appConfigRepository.
            GetAppliedConfigWithFile();
            if (config is null)
            {
                return new AppConfigDto()
                {
                    CompanyName = "Default",
                    Code = $"default:{Guid.NewGuid()}",
                    Id = Guid.NewGuid(),
                    LogoURL = _configuration["Media:DEFAULT_BRAND_URL"],
                    IconURL = _configuration["Media:DEFAULT_ICON_URL"]
                };
            }
            return ObjectMapper.Map<AppConfig,AppConfigDto>(config);
        }

        public async Task<AppConfigDto> CreateAsync(CreateUpdateAppConfigDto input)
        {   
            (input.CompanyName, input.Code) = TrimText(input.CompanyName, input.Code);
            var item = ObjectMapper.Map<CreateUpdateAppConfigDto, AppConfig>(input);

            await _appConfigRepository.AddAsync(item);
            
            return ObjectMapper.Map<AppConfig,AppConfigDto>(item);
        }



        public async Task<AppConfigDto> UpdateAsync(CreateUpdateAppConfigDto input, Guid id)
        {
            (input.CompanyName, input.Code) = TrimText(input.CompanyName, input.Code);

            var item = await _appConfigRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (item is null)
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }
            
            var appConfig = ObjectMapper.Map(input,item);
            appConfig.IsApply = false;
            await _appConfigRepository.UpdateAsync(appConfig);
            return ObjectMapper.Map<AppConfig,AppConfigDto>(appConfig);
        }

 

        public async Task DeleteAsync(Guid id)
        {
            var item = await _appConfigRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (item is null)
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }
            
            _appConfigRepository.Remove(item);
        }

        public async Task ApplyConfig(Guid id)
        {
            
            var configs = await _appConfigRepository.ToListAsync();
            var item = configs.FirstOrDefault(x => x.Id == id);

            if (item is null)
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }
            
            if (item.EnableNotificationByEmail)
            {
                
                //Check connect to Email Host
                var checkConnect = new SendingEmail( new EmailHostConfig()
                {
                    From = item.MailName,
                    Host = item.MailHost,
                    Port = item.MailPort,
                    Password = item.Password,
                    Sender = "BV-Technicians"
                });
               
            }
           
            
            
            foreach (var config in configs)
            {
                config.IsApply = false;
            }
            
            item.IsApply = true;
            _appConfigRepository.UpdateRange(configs);
            
            
      
        }

        public async Task SwitchOffConfig(Guid id)
        {
            var item = await _appConfigRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (item is null)
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }

            item.IsApply = false;
            await _appConfigRepository.UpdateAsync(item);
        }

        public async Task<ApiResponseBase<List<AppConfigDto>>> GetListAsync()
        {

            ApiResponseBase<List<AppConfigDto>> result = new ApiResponseBase<List<AppConfigDto>>();

            try
            {

                var configs = await _appConfigRepository.GetQueryable()
                    .Include(x => x.IconFile)
                    .Include(x => x.LogoFile)
                    .AsSplitQuery().ToListAsync();

                result.Data = ObjectMapper.Map<List<AppConfig>, List<AppConfigDto>>(configs);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }
        
        public async Task<AppConfigDto> AppconfigActiveAsync()
        {

            var config = await _appConfigRepository.GetQueryable()
                .Include(x => x.IconFile)
                .Include(x=>x.LogoFile)
                .AsSplitQuery().FirstOrDefaultAsync(x => x.IsApply == true);

            return ObjectMapper.Map<AppConfigDto>(config);
            
        }
        
        private (string CompanyName, string? Code) TrimText(string name, string code)
        {
            return (name.Trim(), code?.Trim().ToUpper());
        }
    }
}