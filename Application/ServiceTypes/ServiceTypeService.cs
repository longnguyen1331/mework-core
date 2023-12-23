using Contract.ServiceTypes;
using Contract.ServiceTypeServices;
using Core.Const;
using Core.Exceptions;
using Domain.ServiceTypes;
using Microsoft.EntityFrameworkCore;
using SqlServ4r.Repository.Files;
using SqlServ4r.Repository.ServiceTypes;
using System.Net;
using Volo.Abp.DependencyInjection;

namespace Application.ServiceTypes
{
    public class ServiceTypeService : ServiceBase,IServiceTypeService,ITransientDependency
    {
       private readonly ServiceTypeRepository _serviceTypeRepository;
        private readonly StaticFileRepository _staticFileRepository;


        public ServiceTypeService(ServiceTypeRepository serviceTypeRepository,
            StaticFileRepository staticFileRepository)
        {
            _serviceTypeRepository = serviceTypeRepository;
            _staticFileRepository = staticFileRepository;
        }


        public async Task<ServiceTypeDto> CreateAsync(CreateUpdateServiceTypeDto input)
        {
            (input.Name, input.Code) = TrimText(input.Name, input.Code);

            var serviceType = ObjectMapper.Map<CreateUpdateServiceTypeDto, ServiceType>(input);

            if (input.ImageId != null && input.ImageId != Guid.Empty)
            {
                serviceType.ImageFile = await _staticFileRepository.FirstOrDefaultAsync(x => x.Id == input.ImageId);
            }

            await _serviceTypeRepository.AddAsync(serviceType);
            
            return ObjectMapper.Map<ServiceType,ServiceTypeDto>(serviceType);
        }

      
        public async Task<ServiceTypeDto> UpdateAsync(CreateUpdateServiceTypeDto input, Guid id)
        {
            (input.Name, input.Code) = TrimText(input.Name, input.Code);

            var item = await _serviceTypeRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (item is null)
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }
            
            var serviceType = ObjectMapper.Map(input,item);
            if (input.ImageId != null && input.ImageId != Guid.Empty && input.ImageId != serviceType.ImageId)
            {
                serviceType.ImageFile = await _staticFileRepository.FirstOrDefaultAsync(x => x.Id == input.ImageId);
            }

            await _serviceTypeRepository.UpdateAsync(serviceType);
            return ObjectMapper.Map<ServiceType,ServiceTypeDto>(serviceType);
        }

        public async Task DeleteAsync(Guid id)
        {
            var serviceType = await _serviceTypeRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (serviceType is null)
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }
           _serviceTypeRepository.Remove(serviceType);
        }

        public async Task<List<ServiceTypeDto>> GetListAsync()
        {
            var serviceTypes =  _serviceTypeRepository.GetQueryable().
                Include(x=>x.ImageFile).
                OrderBy(x=>x.ODX);
            return ObjectMapper.Map<List<ServiceType>, List<ServiceTypeDto>>(await serviceTypes.ToListAsync());
        }

        public async Task<List<ServiceTypeDto>> GetListAsync(ServiceTypeFilterPagingDto filter)
        {
            var serviceTypes = _serviceTypeRepository.GetQueryable()
                .Include(x => x.ImageFile)
                .Where(x => x.IsDeleted == false && (filter.IsHighlight != null ? filter.IsHighlight == x.IsHighLight : 1 == 1));
                
            if (filter.Take > 0)
                serviceTypes = serviceTypes.Skip(filter.Skip).Take(filter.Take);

            var result = await serviceTypes.OrderBy(x => x.ODX).ToListAsync();

            return ObjectMapper.Map<List<ServiceType>, List<ServiceTypeDto>>(result);
        }
        
        private (string Name, string? Code) TrimText(string name, string code)
        {
            return (name.Trim(), code?.Trim().ToUpper());
        }
    }
}