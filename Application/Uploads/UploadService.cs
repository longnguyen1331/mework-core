using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contract;
using Contract.Files;
using Contract.Uploads;
using Core.Const;
using Core.Exceptions;
using Core.Helper;
using Domain.StaticFiles;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SqlServ4r.Repository.Files;
using Volo.Abp.DependencyInjection;

namespace Application.Uploads
{
    public class UploadService :ServiceBase, IUploadService,ITransientDependency
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly StaticFileRepository _fileRepository;
        
        public UploadService(IConfiguration configuration, IWebHostEnvironment environment,
            StaticFileRepository fileRepository)
        {
            _configuration = configuration;
            _environment = environment;
            _fileRepository = fileRepository;
        }
        

  

        public async Task<Guid> CreateStaticFile(CreateUpdateStaticFileDto input)
        {
            var file = await _fileRepository
                .FirstOrDefaultAsync(x => x.Path == input.Path);

            if (file == null)
            {
                var newFile = ObjectMapper.Map<CreateUpdateStaticFileDto, StaticFile>(input);
                await _fileRepository.AddAsync(newFile);
                return newFile.Id;
            }

            return file.Id;
        }

        public async Task<FileDto> UploadImage(IFormFile file)
        {  
            string pathBase = Path.Combine(_environment.WebRootPath,_configuration["Media:BASE_IMAGE_PATH"]);
            var fileModel = await FileHelper.UploadFile(file, pathBase,new List<string>(){".jpg,.png"}, _configuration["Media:BASE_IMAGE_URL"]);
            return new FileDto(){FileName = fileModel.Name , Path = fileModel.Path,Url = fileModel.Url};
        }

        public async Task<FileDto> UploadExcelFileOfUsers(IFormFile file)
        {
            string pathBase = Path.Combine(_environment.WebRootPath,_configuration["Media:BASE_EXCEL_USER_FILE_PATH"]);
            var fileModel = await FileHelper.UploadFile(file, pathBase,new List<string>(){".csv, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel"});
            return new FileDto(){FileName = fileModel.Name , Path = fileModel.Path,Url = fileModel.Url};
        }
        
        public async Task<FileDto> UploadDocumentFile(IFormFile file)
        {
            string pathBase = Path.Combine(_environment.WebRootPath, _configuration["Media:DOCUMENT_FILE"]);
            var fileModel = await FileHelper.UploadFile(file, pathBase, new List<string>()
            {
                _configuration["DocumentFile:Extension"]
            }, _configuration["Media:BASE_DOCUMENT_FILE_URL"]);

            return new FileDto() { FileName = fileModel.Name, Path = fileModel.Path, Url = fileModel.Url, Extension = fileModel.Extension };
        }

        
        public async Task<UploadFileURLDto> UploadGreetingCard(IFormFile file)
        {
            string pathBase = Path.Combine(_environment.WebRootPath,_configuration["Media:BASE_GreetingCard_PATH"]);
            var fileModel = await FileHelper.UploadFile(file, pathBase,new List<string>()
            {
                ".jpg",".png",".gif"
            }, _configuration["Media:BASE_GreetingCard_URL"]);
            return new UploadFileURLDto()
            {
                URL = fileModel.Url
            };
        }
        
        public async Task<UploadFileURLDto> UploadUserAvatar(IFormFile file)
        {
            string pathBase = Path.Combine(_environment.WebRootPath,_configuration["Media:BASE_Avatar_FILE_PATH"]);
            var fileModel = await FileHelper.UploadFile(file, pathBase,new List<string>()
            {
                ".jpg",".png",".gif"
            }, _configuration["Media:BASE_Avatar_URL"]);
            return new UploadFileURLDto()
            {
                URL = fileModel.Url
            };
        }
        
        public async Task<UploadFileURLDto> UploadUserReviewImg(IFormFile file)
        {
            string pathBase = Path.Combine(_environment.WebRootPath,_configuration["Media:BASE_IMAGE_PATH"]);
            var fileModel = await FileHelper.UploadFile(file, pathBase,new List<string>()
            {
                ".jpg",".png",".gif"
            }, _configuration["Media:BASE_IMAGE_URL"]);
            return new UploadFileURLDto()
            {
                URL = fileModel.Url
            };
        }
        
        
        
        
        //new -version
        public async Task<List<Guid>> UploadImages(List<IFormFile> files)
        {
            var staticfiles = new List<StaticFile>();
            foreach (var item in files)
            {
                string pathBase = Path.Combine(_environment.WebRootPath,_configuration["Media:BASE_IMAGE_PATH"]);
                var fileModel = await FileHelper.UploadFile(item, pathBase,new List<string>(){".jpg,.png"}, _configuration["Media:BASE_IMAGE_URL"]);
                var file = ObjectMapper.Map<FileModel, StaticFile>(fileModel);
                file.FileType = Core.Enum.FileTypes.Image;
                staticfiles.Add(file);
            }
            await _fileRepository.AddRangeAsync(staticfiles);

            return staticfiles.Select(x=>x.Id).ToList();
        }

        public async Task<List<Guid>> UploadTaskFiles(List<IFormFile> files)
        {
            var newFiles = new List<StaticFile>();
            var imgExtensions = new List<string>() {".jpg,.png"};
            var docExtensions = new List<string>() { ".xlsx,.pdf,.docx,.xlsb,.rar, .zip, .pptx, .ppt" };
            string pathBase = Path.Combine(_environment.WebRootPath
                ,_configuration["Media:BASE_TASK_FILE_PATH"]);
            foreach (var item in files)
            {
                var extension = Path.GetExtension(item.FileName);
                if (docExtensions.Any(x=>x.Contains(extension)))
                {
                    var fileModel = await FileHelper
                        .UploadFile(item, pathBase,docExtensions
                            , _configuration["Media:BASE_TASK_FILE_URL"]);
                    
                    var staticFile = ObjectMapper.Map<FileModel, StaticFile>(fileModel);
                    staticFile.FileType = Core.Enum.FileTypes.Document;
                    newFiles.Add(staticFile);
                    
                }else if (imgExtensions.Any(x=>x.Contains(extension)))
                {
                    var fileModel = await FileHelper
                        .UploadFile(item, pathBase,imgExtensions
                            , _configuration["Media:BASE_TASK_FILE_URL"]);
                    
                    var staticFile = ObjectMapper.Map<FileModel, StaticFile>(fileModel);
                    staticFile.FileType = Core.Enum.FileTypes.Image;
                    newFiles.Add(staticFile);
                }
                else
                {
                    throw new GlobalException(HttpMessage.InvalidExtension, HttpStatusCode.BadRequest);
                }
            }
            await _fileRepository.AddRangeAsync(newFiles);
            
            return newFiles.Select(x => x.Id).ToList();
        }

        

        public async Task<Guid> UploadImage1(IFormFile file)
        {  
            string pathBase = Path.Combine(_environment.WebRootPath,_configuration["Media:BASE_IMAGE_PATH"]);
            var fileModel = await FileHelper.UploadFile(file, pathBase,new List<string>(){".jpg,.png"}, _configuration["Media:BASE_IMAGE_URL"]);
            var staticFile = ObjectMapper.Map<FileModel, StaticFile>(fileModel);
            await _fileRepository.AddAsync(staticFile);
            return staticFile.Id;
        }
    }
}