using System;
using System.ComponentModel.DataAnnotations;
using Contract.CustomAttribute;

namespace Contract.DocumentFiles
{
    public class CreateUpdateDocumentFileDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; }

        public DateTime PublicationDate { get; set; } = DateTime.Now;
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public string? Note { get; set; }

        public bool AllowDownloadAndPrint { get; set; } = true;
        public bool IsDeleted { get; set; }

        //media
        public string FileName { get; set; }
        public string Extentions { get; set; }
        public string AbsolutePath { get; set; }
        public string URL { get; set; }


        //foreign key
        [NonEmptyGuid(ErrorMessage = "Please Issuing Agency")]
        public Guid IssuingAgencyId { get; set; }

        [NonEmptyGuid(ErrorMessage = "Please Document Type")]

        public Guid DocumentTypeId { get; set; }

        [NonEmptyGuid(ErrorMessage = "Please Select Folder")]
        public Guid DocumentFolderId { get; set; }


        public Guid? CreatedBy { get; set; }

        public string? CreatedByUserName { get; set; }
        
        
        public Guid IsModify { get; set; }
    }
}