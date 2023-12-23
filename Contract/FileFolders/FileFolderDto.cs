using System;
using System.Collections.Generic;
using Domain.FileFolders;

namespace Contract.FileFolders
{
    public class FileFolderDto
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; }
        public string? Note { get; set; }
        public int ODX { get; set; }
        public Guid? ParentCode { get; set; }
        
        
        //UI
        public List<FileFolderDto> ChildFolders { get; set;}

    }
}