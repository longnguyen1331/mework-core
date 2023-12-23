using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contract.FileFolders;
using WebClient.RequestHttp;

namespace WebClient.Service.FileFolders
{
    public class FileFolderService : IFileFolderService
    {
        public FileFolderService()
        {
            
        }
        
        public async Task<List<FileFolderDto>> GetListAsync()
        {
            return await RequestClient.GetAPIAsync<List<FileFolderDto>>("file-folder");
        }

        public async Task<FileFolderDto> CreateAsync(CreateUpdateFileFolderDto input)
        {
            return await RequestClient.PostAPIAsync<FileFolderDto>("file-folder",input);

        }

        public async Task<FileFolderDto> UpdateAsync(CreateUpdateFileFolderDto input, Guid id)
        {
            return await RequestClient.PutAPIAsync<FileFolderDto>($"file-folder/{id}" , input);

        }

        public async Task DeleteAsync(Guid id)
        { 
            await RequestClient.DeleteAPIAsync<Task>($"file-folder/{id}");
        }
    }
}