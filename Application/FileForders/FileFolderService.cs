using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contract.FileFolders;
using Core.Const;
using Core.Exceptions;
using Domain.FileFolders;
using Microsoft.EntityFrameworkCore;
using SqlServ4r.Repository.FileDocuments;
using SqlServ4r.Repository.FileFolders;
using Volo.Abp.DependencyInjection;

namespace Application.FileForders
{
    public class FileFolderService : ServiceBase, IFileFolderService,ITransientDependency
    {
        private readonly FileFolderRepository _fileFolderRepository;
        private readonly DocumentFileRepository _documentFileRepository;
        public FileFolderService(FileFolderRepository fileFolderRepository,
            DocumentFileRepository documentFileRepository)
        {
            _fileFolderRepository = fileFolderRepository;
            _documentFileRepository = documentFileRepository;
        }
        
        
        public async Task<FileFolderDto> CreateAsync(CreateUpdateFileFolderDto input)
        {   
            (input.Name, input.Code) = TrimText(input.Name, input.Code);

            var folders = ObjectMapper.Map<CreateUpdateFileFolderDto, FileFolder>(input);
            await _fileFolderRepository.AddAsync(folders);
            return ObjectMapper.Map<FileFolder,FileFolderDto>(folders);
        }



        public async Task<FileFolderDto> UpdateAsync(CreateUpdateFileFolderDto input, Guid id)
        {
            (input.Name, input.Code) = TrimText(input.Name, input.Code);

            var item = await _fileFolderRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (item is null)
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }
            
            var folders = ObjectMapper.Map(input,item);
            await _fileFolderRepository.UpdateAsync(folders);

            return ObjectMapper.Map<FileFolder,FileFolderDto>(folders);
        }

 

        public async Task DeleteAsync(Guid id)
        {
            var folder = await _fileFolderRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (folder is null)
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }
            _fileFolderRepository.Remove(folder);
        }

        public async Task<List<FileFolderDto>> GetListAsync()
        {
            var forders = await _fileFolderRepository.GetQueryable()
                .OrderBy(x=>x.ODX).ToListAsync();
               
            return ObjectMapper.Map<List<FileFolder>, List<FileFolderDto>>(forders);
        }
        
        private (string Name, string? Code) TrimText(string name, string code)
        {
            return (name.Trim(), code?.Trim().ToUpper());
        }
    }
}