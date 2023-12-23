using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.FileFolders;

namespace Contract.FileFolders
{
    public interface IFileFolderService
    {
        Task<FileFolderDto> CreateAsync(CreateUpdateFileFolderDto input);
        Task<FileFolderDto> UpdateAsync(CreateUpdateFileFolderDto input,Guid id);
        Task DeleteAsync(Guid id);
        Task<List<FileFolderDto>> GetListAsync();
    }
}