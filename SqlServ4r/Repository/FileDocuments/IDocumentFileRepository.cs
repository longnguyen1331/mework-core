using Contract.DocumentFiles;
using Domain.DocumentFiles;

namespace SqlServ4r.Repository.FileDocuments
{
    public interface IDocumentFileRepository
    {
        Task<List<DocumentFileWithNavProperties>> GetFilesWithNavProperties(DocumentFileFilter filter);
        

    }
}