using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contract.DocumentTypes;
using Core.Const;
using Core.Exceptions;
using Domain.DocumentTypes;
using Microsoft.EntityFrameworkCore;
using SqlServ4r.Repository.DocumentTypes;
using Volo.Abp.DependencyInjection;

namespace Application.FileTypes
{
    public class DocumentTypeService : ServiceBase,IDocumentTypeService,ITransientDependency
    {
         private readonly DocumentTypeRepository _documentTypeRepository;
        public DocumentTypeService(DocumentTypeRepository documentFolderRepository)
        {
            _documentTypeRepository = documentFolderRepository;
        }
        
        
        public async Task<DocumentTypeDto> CreateAsync(CreateUpdateDocumentTypeDto input)
        {   
            (input.Name, input.Code) = TrimText(input.Name, input.Code);

            var types = ObjectMapper.Map<CreateUpdateDocumentTypeDto, DocumentType>(input);
            await _documentTypeRepository.AddAsync(types);
            return ObjectMapper.Map<DocumentType,DocumentTypeDto>(types);
        }



        public async Task<DocumentTypeDto> UpdateAsync(CreateUpdateDocumentTypeDto input, Guid id)
        {
            (input.Name, input.Code) = TrimText(input.Name, input.Code);

            var item = await _documentTypeRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (item is null)
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }
            
            var types = ObjectMapper.Map(input,item);
            await _documentTypeRepository.UpdateAsync(types);

            return ObjectMapper.Map<DocumentType,DocumentTypeDto>(types);
        }

 

        public async Task DeleteAsync(Guid id)
        {
            var types = await _documentTypeRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (types is null)
            {
                throw new GlobalException(HttpMessage.NotFound, HttpStatusCode.BadRequest);
            }
            
            _documentTypeRepository.Remove(types);
        }

        public async Task<List<DocumentTypeDto>> GetListAsync()
        {
            var types = await _documentTypeRepository.GetQueryable()
                .OrderBy(x=>x.ODX).ToListAsync();
               
            return ObjectMapper.Map<List<DocumentType>, List<DocumentTypeDto>>(types);
        }
        
        private (string Name, string? Code) TrimText(string name, string code)
        {
            return (name.Trim(), code?.Trim().ToUpper());
        }
    }
}