using System;
using Core.Enum;

namespace Contract.Files
{
    public class StaticFileDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
        public FileTypes FileType { get; set; } = FileTypes.Unknown;
    }
}