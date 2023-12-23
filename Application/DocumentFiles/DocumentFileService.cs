using Contract.DocumentFiles;
using Core.Const;
using Core.Enum;
using Core.Exceptions;
using Core.Helper;
using Domain.DocumentFiles;
using Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SqlServ4r.Repository.FileDocuments;
using SqlServ4r.Repository.Users;
using System.Net;
using Volo.Abp.DependencyInjection;

namespace Application.DocumentFiles
{
    public class DocumentFileService :ServiceBase,IDocumentFileService,ITransientDependency
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly UserRepository _userRepository;
        private readonly DocumentFileRepository _documentFileRepository;

        public DocumentFileService(DocumentFileRepository documentFileRepository,
            UserManager<User> userManager,
            UserRepository userRepository,
            IConfiguration configuration
        )
        {
            _documentFileRepository = documentFileRepository;
            _userManager = userManager;
            _userRepository = userRepository;
            _configuration = configuration;
        }


        public  async Task<List<DocumentFileWithNavPropertiesDto>> GetListWithNavPropertiesAsync(DocumentFileFilter filter)
        {
            var filesWithNavProperties = await _documentFileRepository.GetFilesWithNavProperties(filter);
            
            return ObjectMapper.Map<List<DocumentFileWithNavProperties>,
                List<DocumentFileWithNavPropertiesDto>>(filesWithNavProperties);
        }

   

        public async Task<List<DocumentFileDto>> GetListAsync()
        {
            var files = await _documentFileRepository.ToListAsync();
            
            return ObjectMapper.Map<List<DocumentFile>,
                List<DocumentFileDto>>(files);;
        }

      
        public async Task<DocumentFileDto> GetAsync(Guid id)
        {
            var item = await  _documentFileRepository.GetAsync(x => x.Id == id);
            if (item is null) throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            return ObjectMapper.Map<DocumentFile,DocumentFileDto>(item);
        }

        public async Task<DocumentFileDto> CreateAsync(CreateUpdateDocumentFileDto input)
        {
            var storageCode = _configuration["DocumentFile:StorageCode"];
            var user = await _userManager.FindByNameAsync(input.CreatedByUserName);
            var file = ObjectMapper.Map<CreateUpdateDocumentFileDto, DocumentFile>(input);
            file.StorageCode = GetStorageCodeFromConfigCode(storageCode);
            file.CreatedBy = user.Id; 
            
            await _documentFileRepository.AddAsync(file);
            return ObjectMapper.Map<DocumentFile, DocumentFileDto>(file);
        }


        private string GetStorageCodeFromConfigCode(string configCode)
        {
            var tags =  configCode.Split(new[] {'[', ']'});
            var storageCode = "";
           
            foreach (string item in tags)
            {
                if (!item.IsNullOrWhiteSpace())
                {
                    var ticket = item;
                    ticket = item.ToUpper();
                    switch (ticket)
                    {
                        case "YEAR":
                        {
                            storageCode+= $"{DateTime.Now.Year}";
                            break;
                        }
                        case "MONTH":
                        {
                            storageCode+= $"{DateTime.Now.Month}";
                            break;
                        }
                        case "DAY":
                        {
                            storageCode+= $"{DateTime.Now.Day}{DateTime.Now.Hour}{DateTime.Now.Millisecond}";
                            break;
                        }
                        
                        case "RANDOMCODE":
                        {
                            storageCode += RandomCodeHelper.GenerateRandomCode(0, 10, 4);
                            break;
                        }
                        default:
                        {
                            storageCode += ticket;
                            break;
                        }
                     
                    }
              
                    
                    
                }
            }

            return storageCode;
        }
        
        

        public async Task<DocumentFileDto> UpdateAsync(CreateUpdateDocumentFileDto input, Guid id)
        {
            var item = await _documentFileRepository.FirstOrDefaultAsync(x => x.Id == id);
            
            if(item is null)  throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            
            var file = ObjectMapper.Map(input,item);
            await _documentFileRepository.UpdateAsync(file);

            var user =  await _userRepository
                .FirstOrDefaultAsync(x => x.Id == input.IsModify);
            
            
            return ObjectMapper.Map<DocumentFile, DocumentFileDto>(file);
        }
        
        public async Task DeleteAsync(Guid id)
        {
            var item = await _documentFileRepository.FirstOrDefaultAsync(x => x.Id == id);
            if(item is null)  throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            item.IsDeleted = true;
            await _documentFileRepository.UpdateAsync(item);
            
            
            
            
        }

        public async Task DeleteByOptionsAsync(SelectedDeletionRequestDto request)
        {
            if (request.Option == FileDeletionOption.File)
            {
               await DeleteAsync(request.FileId);
            }
        }

        public async Task UpdateDownloadCountAsync(Guid id)
        {
            var item = await _documentFileRepository.FirstOrDefaultAsync(x => x.Id == id);
            if(item is null)  throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            item.DownloadCount += 1;
            await _documentFileRepository.UpdateAsync(item);
        }

        public async Task UpdatePrintCountAsync(Guid id)
        {
            var item = await _documentFileRepository.FirstOrDefaultAsync(x => x.Id == id);
            if(item is null)  throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            item.PrintCount += 1;
            await _documentFileRepository.UpdateAsync(item);
        }
    }
}