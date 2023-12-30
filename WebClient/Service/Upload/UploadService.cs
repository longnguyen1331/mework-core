using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contract.Files;
using Contract.Uploads;
using Core.Helper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using WebClient.RequestHttp;

namespace WebClient.Service.Upload
{
    public class UploadService 
    {
        public UploadService()
        {
            
        }
        public async Task<Guid> CreateStaticFile(CreateUpdateStaticFileDto input)
        {
            return  await RequestClient.PostAPIAsync<Guid>("upload/create-static-file", input);
        }
        public async Task<FileDto> UploadImage(IBrowserFile file)
        {
            return  await RequestClient.PostAPIWithFileAsync<FileDto>("upload/save-image", file);
        }
        public async Task<List<Guid>> UploadImages(List<IBrowserFile> files)
        {
            return  await RequestClient.PostAPIWithMultipleFileAsync<List<Guid>>("upload/save-images", files);
        }
        
        public async Task<List<Guid>> UploadTaskFiles(List<IBrowserFile> files)
        {
            return  await RequestClient.PostAPIWithMultipleFileAsync<List<Guid>>("upload/save-task-files", files);
        }
        
        
        public async Task<UploadFileURLDto> UploadGreetingCard(IBrowserFile file)
        {
            return  await RequestClient.PostAPIWithFileAsync<UploadFileURLDto>("upload/save-greeting-card", file);
        }
        
        public async Task<Guid> UploadImage1(IBrowserFile file)
        {
            return  await RequestClient.PostAPIWithFileAsync<Guid>("upload/save-image1", file);
        }
       
        //old version
        public async Task<FileDto> UploadExcelFileOfUsers(IBrowserFile file)
        {
            return  await RequestClient.PostAPIWithFileAsync<FileDto>("upload/save-excel-file-of-users", file);
        }

        public async Task<FileDto> UploadDocumentFile(IBrowserFile file)
        {
            return  await RequestClient.PostAPIWithFileAsync<FileDto>("upload/save-document-file", file);
        }

        
        public async Task<UploadFileURLDto> UploadUserAvatar(IBrowserFile file)
        {
            return  await RequestClient.PostAPIWithFileAsync<UploadFileURLDto>("upload/save-user-avatar", file);
        }

    }
}