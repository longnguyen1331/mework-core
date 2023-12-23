using System;
using Domain.DocumentFiles;
using Domain.Identity.Users;

namespace Domain.FileVersions
{
    public class FileVersion
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public string? Note { get; set; }
        
        
        
        //foreign key
        public Guid FileId { get; set; }
        public Guid EditBy { get; set; }

        
        //Navigation
        public DocumentFile DocumentFile { get; set; }
        public User User { get; set; }
    }
}