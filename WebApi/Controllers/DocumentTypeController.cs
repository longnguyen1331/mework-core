using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.FileForders;
using Application.FileTypes;
using Contract.DocumentTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/document-type/")]
    [Authorize]
    public class DocumentTypeController
    {
        private DocumentTypeService _documentTyperService;
        
        public DocumentTypeController(DocumentTypeService documentFolderService)
        {
            _documentTyperService = documentFolderService;
        }
        
        [HttpPost]
        public async Task<DocumentTypeDto> CreateAsync(CreateUpdateDocumentTypeDto input)
        {
            return await _documentTyperService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<DocumentTypeDto> UpdateAsync(CreateUpdateDocumentTypeDto input, Guid id)
        {
            return  await _documentTyperService.UpdateAsync(input,id);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteAsync(Guid id)
        {
            await _documentTyperService.DeleteAsync(id);
        }

        [HttpGet]
        public async Task<List<DocumentTypeDto>> GetListAsync()
        {
            return await _documentTyperService.GetListAsync();
        }
    }
}