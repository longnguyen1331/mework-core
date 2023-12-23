using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Uploads;
using Castle.Core.Configuration;
using Contract;
using Contract.Files;
using Contract.Uploads;
using Core.Helper;
using Domain.StaticFiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/upload/")]
    public class UploadController 
    {
        private UploadService _uploadService;

        public UploadController(UploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [HttpPost]
        [Route("create-static-file")]
        public async Task<Guid> CreateStaticFile(CreateUpdateStaticFileDto input)
        {
            return await _uploadService.CreateStaticFile(input);
        }

        [HttpPost]
        [Route("save-image")]
        public async Task<FileDto> UploadImage([FromForm] IFormFile file)
        {
            return await _uploadService.UploadImage(file);
        }
        
        [HttpPost]
        [Route("save-images")]
        public async Task<List<Guid>> UploadImages([FromForm]List<IFormFile> files)
        {
            return await _uploadService.UploadImages(files);
        }
        
        [HttpPost]
        [Route("save-task-files")]
        public async Task<List<Guid>> UploadTaskFiles([FromForm] List<IFormFile> files)
        {
            return await _uploadService.UploadTaskFiles(files);
        }
        
        
        [HttpPost]
        [Route("save-user-avatar")]
        public async Task<UploadFileURLDto> UploadUserAvatar([FromForm] IFormFile file)
        {
            return await _uploadService.UploadUserAvatar(file);
        }
        
        
        [HttpPost]
        [Route("save-greeting-card")]
        public async Task<UploadFileURLDto> UploadGreetingCard([FromForm] IFormFile file)
        {
            return await _uploadService.UploadGreetingCard(file);
        }

        
        

        [HttpPost]
        [Route("save-user-review-img")]
        public async Task<UploadFileURLDto> UploadUserReviewImg(IFormFile file)
        {
            return await _uploadService.UploadUserReviewImg(file);
        }



        [HttpPost]
        [Route("save-image1")]
        public async Task<Guid> UploadImage1([FromForm] IFormFile file)
        {
            return await _uploadService.UploadImage1(file);
        }
        
        
        
        

        [HttpPost]
        [Route("save-excel-file-of-users")]
        public async Task<FileDto> UploadExcelFileOfUsers(IFormFile file)
        {
            return await _uploadService.UploadExcelFileOfUsers(file);
        }

        [HttpPost]
        [Route("save-document-file")]
        public async Task<FileDto> UploadDocumentFile(IFormFile file)
        {
            return await _uploadService.UploadDocumentFile(file);
        }
        
        
      
        
        
    }
}