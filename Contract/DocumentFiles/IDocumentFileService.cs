namespace Contract.DocumentFiles
{
    public interface IDocumentFileService
    {
        Task<List<DocumentFileWithNavPropertiesDto>> GetListWithNavPropertiesAsync(DocumentFileFilter filter);

        Task<List<DocumentFileDto>> GetListAsync();
        Task<DocumentFileDto> GetAsync(Guid id);
        Task<DocumentFileDto> CreateAsync(CreateUpdateDocumentFileDto input);
        Task<DocumentFileDto> UpdateAsync(CreateUpdateDocumentFileDto input,Guid id);
        Task DeleteAsync(Guid id);
        Task DeleteByOptionsAsync(SelectedDeletionRequestDto request);
        Task UpdateDownloadCountAsync(Guid id);
        Task UpdatePrintCountAsync(Guid id);
    }
}