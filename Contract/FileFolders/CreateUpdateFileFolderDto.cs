using System;

namespace Contract.FileFolders
{
    public class CreateUpdateFileFolderDto
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; }
        public string? Note { get; set; }
        public int ODX { get; set; }
        public Guid? ParentCode { get; set; }

    }
}