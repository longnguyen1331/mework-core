using System;
using Core.Enum;

namespace Contract.Files
{
    public class StaticFileDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string URL { get; set; }
        public FileTypes FileType { get; set; } = FileTypes.Unknown;
    }
}