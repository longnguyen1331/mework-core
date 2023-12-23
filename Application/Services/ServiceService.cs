using Contract;
using Contract.Services;
using Core.Const;
using Core.Enum;
using Core.Exceptions;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using SqlServ4r.Repository.Files;
using SqlServ4r.Repository.Services;
using SqlServ4r.Repository.ServiceTypes;
using System.Net;
using Volo.Abp.DependencyInjection;

namespace Application.Services
{
    public class ServiceService : ServiceBase, IServiceService, ITransientDependency
    {
        private readonly ServiceRepository _serviceRepository;
        private readonly StaticFileRepository _staticFileRepository;
        private readonly ServiceTypeRepository _serviceTypeRepository;

        public ServiceService(ServiceRepository serviceRepository,
                              StaticFileRepository staticFileRepository,
                              ServiceTypeRepository serviceTypeRepository)
        {
            _serviceRepository = serviceRepository ?? throw new ArgumentNullException(nameof(serviceRepository));
            _staticFileRepository = staticFileRepository ?? throw new ArgumentNullException(nameof(staticFileRepository));
            _serviceTypeRepository = serviceTypeRepository ?? throw new ArgumentNullException(nameof(serviceTypeRepository));
        }


        public async Task<ApiResponseBase<ServiceDto>> CreateAsync(CreateUpdateServiceDto input)
        {
            ApiResponseBase<ServiceDto> result = new ApiResponseBase<ServiceDto>();
            try
            {
                (input.Name, input.Code) = TrimText(input.Name, input.Code);

                var service = ObjectMapper.Map<CreateUpdateServiceDto, Service>(input);


                await _serviceRepository.AddAsync(service);

                result.Data = ObjectMapper.Map<Service, ServiceDto>(service);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }


        public async Task<ApiResponseBase<ServiceDto>> UpdateAsync(CreateUpdateServiceDto input, Guid id)
        {
            ApiResponseBase<ServiceDto> result = new ApiResponseBase<ServiceDto>();
            try
            {

                (input.Name, input.Code) = TrimText(input.Name, input.Code);

                var item = await _serviceRepository.FirstOrDefaultAsync(x => x.Id == id);

                if (item is null)
                {
                    throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
                }

                var service = ObjectMapper.Map(input, item);
                if (input.ImageId != null && input.ImageId != Guid.Empty && input.ImageId != service.ImageId)
                {
                    service.ImageFile = await _staticFileRepository.FirstOrDefaultAsync(x => x.Id == input.ImageId);
                }

                await _serviceRepository.UpdateAsync(service);
                result.Data = ObjectMapper.Map<Service, ServiceDto>(service);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ApiResponseBase<ServiceDto>> DeleteAsync(Guid id)
        {
            ApiResponseBase<ServiceDto> result = new ApiResponseBase<ServiceDto>();
            try
            {
                var service = await _serviceRepository.FirstOrDefaultAsync(x => x.Id == id);

                if (service is null)
                {
                    throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
                }
                service.IsDeleted = true;
                await _serviceRepository.UpdateAsync(service);
                result.Data = ObjectMapper.Map<Service, ServiceDto>(service);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ApiResponseBase<ServiceDto>> GetByIdAsync(Guid id)
        {
            ApiResponseBase<ServiceDto> result = new ApiResponseBase<ServiceDto>();
            try
            {
                var services = await (from service in _serviceRepository.GetQueryable()
                                      join serviceType in _serviceTypeRepository.GetQueryable()
                                        on service.ServiceTypeId equals serviceType.Id
                                      join image in _staticFileRepository.GetQueryable()
                                        on service.ImageId equals image.Id
                                      where service.Id == id
                                      select new ServiceDto
                                      {
                                          Id = service.Id,
                                          Code = service.Code,
                                          Name = service.Name,
                                          ODX = service.ODX,
                                          ImageUrl = image.URL,
                                          ImageId = image.Id,
                                          ServiceTypeId = serviceType.Id,
                                          ServiceTypeName = serviceType.Name,
                                          GenderText = ((Gender)service.Gender).ToString(),
                                          Charges = service.Charges,
                                          OldCharges = service.OldCharges,
                                          Description = service.Description,
                                          Tags = service.Tags,
                                          SeoTitle = service.SeoTitle,
                                          Slug = service.Slug,
                                          SeoKeyword = service.SeoKeyword,
                                          SeoDescription = service.SeoDescription,
                                          Note = service.Note,
                                          Views = service.Views,
                                          IsDeleted = service.IsDeleted,
                                          IsHighLight = service.IsHighLight,
                                          IsVisibled = service.IsVisibled,
                                      }).OrderBy(x => x.ODX).AsNoTracking().FirstOrDefaultAsync();
                result.Data = services;

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ApiResponseBase<List<ServiceDto>>> GetListAsync()
        {
            ApiResponseBase<List<ServiceDto>> result = new ApiResponseBase<List<ServiceDto>>();
            try
            {
                var services = await (from service in _serviceRepository.GetQueryable()
                                      join serviceType in _serviceTypeRepository.GetQueryable()
                                        on service.ServiceTypeId equals serviceType.Id
                                      join image in _staticFileRepository.GetQueryable()
                                        on service.ImageId equals image.Id
                                      where service.IsDeleted == false
                                      select new ServiceDto
                                      {
                                          Id = service.Id,
                                          Code = service.Code,
                                          Name = service.Name,
                                          ODX = service.ODX,
                                          ImageUrl = image.URL,
                                          ImageId = image.Id,
                                          ServiceTypeId = serviceType.Id,
                                          ServiceTypeName = serviceType.Name,
                                          GenderText = ((Gender)service.Gender).ToString(),
                                          Charges = service.Charges,
                                          OldCharges = service.OldCharges,
                                          Description = service.Description,
                                          Tags = service.Tags,
                                          SeoTitle = service.SeoTitle,
                                          Slug = service.Slug,
                                          SeoKeyword = service.SeoKeyword,
                                          SeoDescription = service.SeoDescription,
                                          Note = service.Note,
                                          Views = service.Views,
                                          IsDeleted = service.IsDeleted,
                                          IsHighLight = service.IsHighLight,
                                          IsVisibled = service.IsVisibled,
                                      }).OrderBy(x => x.ODX).AsNoTracking().ToListAsync();


                result.Data = services;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ApiResponseBase<List<ServiceDto>>> GetListPagingAsync(BaseFilterPagingDto filter)
        {
            ApiResponseBase<List<ServiceDto>> result = new ApiResponseBase<List<ServiceDto>>();
            try
            {
                var services = await (from service in _serviceRepository.GetQueryable()
                                      join serviceType in _serviceTypeRepository.GetQueryable()
                                        on service.ServiceTypeId equals serviceType.Id into serviceTypes
                                      from svt in serviceTypes.DefaultIfEmpty()
                                      join image in _staticFileRepository.GetQueryable()
                                        on service.ImageId equals image.Id into imgss
                                      from imgs in imgss.DefaultIfEmpty()
                                      where service.IsDeleted == false
                                      && (
                                        string.IsNullOrEmpty(filter.FilterText)
                                            ? 1 == 1
                                            : (!string.IsNullOrEmpty(filter.FilterText) && service.Name.Trim().ToLower() == filter.FilterText.Trim().ToLower())
                                        )

                                      select new ServiceDto
                                      {
                                          Id = service.Id,
                                          Code = service.Code,
                                          Name = service.Name,
                                          ODX = service.ODX,
                                          ImageUrl = imgs.URL,
                                          ImageId = imgs.Id,
                                          ServiceTypeId = svt.Id,
                                          ServiceTypeName = svt.Name,
                                          GenderText = ((Gender)service.Gender).ToString(),
                                          Charges = service.Charges,
                                          OldCharges = service.OldCharges,
                                          Description = service.Description,
                                          Tags = service.Tags,
                                          SeoTitle = service.SeoTitle,
                                          Slug = service.Slug,
                                          SeoKeyword = service.SeoKeyword,
                                          SeoDescription = service.SeoDescription,
                                          Note = service.Note,
                                          Views = service.Views,
                                          IsDeleted = service.IsDeleted,
                                          IsHighLight = service.IsHighLight,
                                          IsVisibled = service.IsVisibled,
                                      }).OrderBy(x => x.ODX)
                                      .Skip(filter.Skip)
                                      .Take(filter.Take).AsNoTracking().ToListAsync();


                result.Data = services;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ApiResponseBase<ServiceSearchResultDto>> GetListPagingAsync(ServiceFilterPagingDto filter)
        {
            ApiResponseBase<ServiceSearchResultDto> result = new ApiResponseBase<ServiceSearchResultDto>();
            try
            {
                List<string> tags = filter.Tag != null && !string.IsNullOrEmpty(filter.Tag) ? filter.Tag.Split(',').ToList() : new List<string>();
                List<string> iCD10s = filter.ICD10 != null && !string.IsNullOrEmpty(filter.ICD10) ? filter.ICD10.Split(',').ToList() : new List<string>();

                result.Data = new ServiceSearchResultDto();

                var services = (from service in _serviceRepository.GetQueryable().AsNoTracking()
                                join serviceType in _serviceTypeRepository.GetQueryable().AsNoTracking()
                                  on service.ServiceTypeId equals serviceType.Id into serviceTypes
                                from svt in serviceTypes.DefaultIfEmpty()
                                join image in _staticFileRepository.GetQueryable().AsNoTracking()
                                  on service.ImageId equals image.Id into imgss
                                from imgs in imgss.DefaultIfEmpty()
                                where service.IsDeleted == false
                                && (!string.IsNullOrEmpty(filter.FilterText) ? !string.IsNullOrEmpty(service.Name) && service.Name.Trim().ToLower().Contains(filter.FilterText.Trim().ToLower()) : 1 == 1)
                                && (!string.IsNullOrEmpty(filter.ServiceTypeCode) ? !string.IsNullOrEmpty(svt.Code) && svt.Code.Trim().ToLower().Equals(filter.ServiceTypeCode.Trim().ToLower()) : 1 == 1)
                                && (!string.IsNullOrEmpty(filter.ServiceSlug) ? (!string.IsNullOrEmpty(service.Slug) && service.Slug.Trim().ToLower().Equals(filter.ServiceSlug.Trim().ToLower()) || !string.IsNullOrEmpty(service.Code) && service.Code.Trim().ToLower().Equals(filter.ServiceSlug.Trim().ToLower())) : 1 == 1)
                                && (filter.IsHighLight != null ? service.IsHighLight == filter.IsHighLight : 1 == 1)
                                && (filter.ServiceTypeId != null && filter.ServiceTypeId != Guid.Empty ? service.ServiceTypeId == filter.ServiceTypeId : 1 == 1)
                                && (filter.IgnoreServiceId == null || filter.IgnoreServiceId == Guid.Empty ? 1 == 1 : service.Id != filter.IgnoreServiceId)
                                select new ServiceDto
                                {
                                    Id = service.Id,
                                    Code = service.Code,
                                    Name = service.Name,
                                    ODX = service.ODX,
                                    ImageUrl = imgs.URL,
                                    ImageId = imgs.Id,
                                    ServiceTypeId = svt.Id,
                                    ServiceTypeName = svt.Name,
                                    GenderText = ((Gender)service.Gender).ToString(),
                                    Charges = service.Charges,
                                    OldCharges = service.OldCharges,
                                    Description = service.Description,
                                    Tags = service.Tags,
                                    SeoTitle = service.SeoTitle,
                                    Slug = service.Slug,
                                    SeoKeyword = service.SeoKeyword,
                                    SeoDescription = service.SeoDescription,
                                    Note = service.Note,
                                    Views = service.Views,
                                    IsDeleted = service.IsDeleted,
                                    IsHighLight = service.IsHighLight,
                                    IsVisibled = service.IsVisibled,
                                }).AsEnumerable();

                if (filter.IsTopViews == true)
                    services = services.OrderByDescending(x => x.Views).ThenBy(x => x.ODX).AsEnumerable();
                else
                    services = services.OrderBy(x => x.ODX).AsEnumerable();

                //.Skip(filter.Skip)
                //.Take(filter.Take).AsNoTracking().ToListAsync();

                if (tags.Any())
                    services = services.Where(x => x.Tags.Split(',').Intersect(tags).Count() > 0);
                if (iCD10s.Any())
                    services = services.Where(x => x.Icd10.Split(',').Intersect(iCD10s).Count() > 0);

                result.Data.TotalItems = services.Count();

                if (filter.Take > 0)
                    services = services.Skip(filter.Skip).Take(filter.Take);

                result.Data.Items = services.ToList();
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ApiResponseBase<List<ServiceDto>>> GetRelatedListAsync(Guid id, int take = 10)
        {
            ApiResponseBase<List<ServiceDto>> result = new ApiResponseBase<List<ServiceDto>>();
            try
            {
                var curService = await _serviceRepository.FirstOrDefaultAsync(x => x.Id == id);

                if (curService == null)
                {
                    result.Data = new List<ServiceDto>();

                    return result;
                }

                var services = await _serviceRepository
                    .GetQueryable()
                    .Include(x => x.ImageFile)
                    .Include(x => x.ServiceType)
                    .Where(x => x.IsDeleted == false && (!string.IsNullOrEmpty(x.Tags) || x.ServiceTypeId == curService.ServiceTypeId) && x.Id != id)
                    .OrderByDescending(x => x.CreatedDate)
                    .OrderBy(x => x.ODX)
                    .ToListAsync();

                List<string> tags = curService.Tags != null && !string.IsNullOrEmpty(curService.Tags) ? curService.Tags.Split(',').ToList() : new List<string>();

                result.Data = services
                    .Where(x => (!string.IsNullOrEmpty(x.Tags) && x.Tags.Split(",", StringSplitOptions.None).Intersect(tags).Count() > 0 || x.ServiceTypeId == curService.ServiceTypeId)).Take(take > 0 ? take : int.MaxValue)
                    .Select(service => new ServiceDto
                    {
                        Id = service.Id,
                        Code = service.Code ?? string.Empty,
                        Name = service.Name ?? string.Empty,
                        ODX = service.ODX,
                        ImageUrl = service.ImageFile?.URL ?? string.Empty,
                        ImageId = service.ImageFile?.Id,
                        ServiceTypeId = service.ServiceType.Id,
                        ServiceTypeName = service.ServiceType.Name,
                        GenderText = ((Gender)service.Gender).ToString(),
                        Charges = service.Charges,
                        OldCharges = service.OldCharges,
                        Description = service.Description ?? string.Empty,
                        Tags = service.Tags ?? string.Empty,
                        SeoTitle = service.SeoTitle ?? string.Empty,
                        Slug = service.Slug ?? string.Empty,
                        SeoKeyword = service.SeoKeyword ?? string.Empty,
                        SeoDescription = service.SeoDescription ?? string.Empty,
                        Note = service.Note ?? string.Empty,
                        Views = service.Views,
                        IsDeleted = service.IsDeleted,
                        IsHighLight = service.IsHighLight,
                        IsVisibled = service.IsVisibled,
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        private (string Name, string? Code) TrimText(string name, string code)
        {
            return (name.Trim(), code?.Trim().ToUpper());
        }

        public async Task<ApiResponseBase<int>> IncreaseViewsTotalAsync(Guid id)
        {
            ApiResponseBase<int> result = new ApiResponseBase<int>();

            try
            {
                var service = await _serviceRepository.GetQueryable().FirstOrDefaultAsync(x => x.Id == id);

                if (service != null)
                {
                    service.Views++;

                    await _serviceRepository.UpdateAsync(service);

                    result.Data = service.Views;
                }
                result.Message = "Service could not be found!";
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}