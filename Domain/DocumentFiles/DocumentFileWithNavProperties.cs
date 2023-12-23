using Domain.DocumentTypes;

namespace Domain.DocumentFiles
{
    public class DocumentFileWithNavProperties
    {
        public DocumentFile File { get; set; }
        public DocumentType DocumentType { get; set; }
        
        public bool IsSentFile { get; set; } = false;
    }
}