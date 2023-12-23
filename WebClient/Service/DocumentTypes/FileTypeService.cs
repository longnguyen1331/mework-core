using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contract.DocumentTypes;
using WebClient.RequestHttp;

namespace WebClient.Service.DocumentTypes
{
    public class DocumentTypeService
    {
        public DocumentTypeService()
        {
            
        }
        
        public async Task<List<DocumentTypeDto>> GetListAsync()
        {
            return await RequestClient.GetAPIAsync<List<DocumentTypeDto>>("document-type");
        }

        public async Task<DocumentTypeDto> CreateAsync(CreateUpdateDocumentTypeDto input)
        {
            return await RequestClient.PostAPIAsync<DocumentTypeDto>("document-type",input);

        }

        public async Task<DocumentTypeDto> UpdateAsync(CreateUpdateDocumentTypeDto input, Guid id)
        {
            return await RequestClient.PutAPIAsync<DocumentTypeDto>($"document-type/{id}" , input);

        }

        public async Task DeleteAsync(Guid id)
        { 
            await RequestClient.DeleteAPIAsync<Task>($"document-type/{id}");
        }
    }
}