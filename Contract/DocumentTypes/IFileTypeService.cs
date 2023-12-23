using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contract.DocumentTypes
{
    public interface IDocumentTypeService
    {
        Task<DocumentTypeDto> CreateAsync(CreateUpdateDocumentTypeDto input);
        Task<DocumentTypeDto> UpdateAsync(CreateUpdateDocumentTypeDto input,Guid id);
        Task DeleteAsync(Guid id);
        Task<List<DocumentTypeDto>> GetListAsync();
    }
}