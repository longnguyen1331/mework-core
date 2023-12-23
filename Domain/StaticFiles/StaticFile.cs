using Core.Enum;
using Domain.AppConfigs;

namespace Domain.StaticFiles
{
    public class StaticFile
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Extension { get; set; }
        public string URL { get; set; }
        public string Path { get; set; }
        public FileTypes FileType { get; set; } = FileTypes.Unknown;

        public List<AppConfig> AppConfigLogos { get; set; }
        
        public List<AppConfig> AppConfigIcons { get; set; }
    }
}