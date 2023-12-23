using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contract.Files;
using Core.Helper;
using Microsoft.AspNetCore.Http;

namespace Contract.Uploads
{
    public interface IUploadService
    {
        public Task<Guid> CreateStaticFile(CreateUpdateStaticFileDto input);
        public Task<FileDto>   UploadImage(IFormFile file);
        
        public Task<FileDto>   UploadExcelFileOfUsers(IFormFile file);

        public Task<FileDto> UploadDocumentFile(IFormFile file);
        
        public Task<List<Guid>> UploadImages(List<IFormFile> files);

        public Task<List<Guid>> UploadTaskFiles(List<IFormFile> files);

    }
}