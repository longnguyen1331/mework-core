using Contract.DocumentTypes;

namespace Contract.DocumentFiles
{
    public class DocumentFileWithNavPropertiesDto
    {
        public DocumentFileDto File { get; set; }
        public DocumentTypeDto DocumentType { get; set; }
        public bool IsSentFile { get; set; } 
    }
}