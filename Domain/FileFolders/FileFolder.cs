using System;
using System.Collections.Generic;
using Domain.DocumentFiles;

namespace Domain.FileFolders
{
    public class FileFolder
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; }
        public string? Note { get; set; }
        public int ODX { get; set; }
        public Guid? ParentCode { get; set; }

        //navigation

        public List<DocumentFile> Files { get; set;}
    }
}