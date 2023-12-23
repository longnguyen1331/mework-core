using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Departments;
using Application.FileForders;
using Contract.Departments;
using Contract.FileFolders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/file-folder/")]
    [Authorize]
    public class FileFolderController : IFileFolderService
    {
        private FileFolderService _fileFolderService;
        
        public FileFolderController(FileFolderService fileFolderService)
        {
            _fileFolderService = fileFolderService;
        }
        
        [HttpPost]
        public async Task<FileFolderDto> CreateAsync(CreateUpdateFileFolderDto input)
        {
            return await _fileFolderService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<FileFolderDto> UpdateAsync(CreateUpdateFileFolderDto input, Guid id)
        {
            return  await _fileFolderService.UpdateAsync(input,id);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _fileFolderService.DeleteAsync(id);
        }

        [HttpGet]
        public async Task<List<FileFolderDto>> GetListAsync()
        {
            return await _fileFolderService.GetListAsync();
        }
    }
}