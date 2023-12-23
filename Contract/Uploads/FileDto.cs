using System;

namespace Contract.Uploads
{
    public class FileDto
    {
        public string Path { get; set; }
        public string FileName { get; set;}
        
        public string? Url { get; set; }
        
        public string? Extension { get; set; }
    }
    
    
}