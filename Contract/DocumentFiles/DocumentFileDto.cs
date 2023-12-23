using System;
using System.Collections.Generic;
using Core.Enum;
using Domain.FileFolders;
using Domain.FileVersions;
using Domain.Identity.Users;

namespace Contract.DocumentFiles
{
    public class DocumentFileDto
    {
        public Guid Id { get; set; }


        public string Name { get; set;}
        public string Code { get; set; }
        
        public DateTime PublicationDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string? Note { get; set; }
        
        
        public string StorageCode { get; set; }
        public bool AllowDownloadAndPrint { get; set; }
        public bool IsDeleted { get; set; }
        public int Views { get; set; }
        public int DownloadCount { get; set; }
        public int PrintCount { get; set; }        
        
        //media 
        public Guid?  StaticFileId { get; set; }
        public string FileName { get; set; }
        public string Extentions { get; set; } 
        public string AbsolutePath { get; set; }
        public string URL { get; set; }

        //foreign key
        public Guid IssuingAgencyId { get; set; }
        public Guid DocumentTypeId { get; set; }
        public Guid DocumentFolderId { get; set; }
        public Guid CreatedBy { get; set; }



    }
}