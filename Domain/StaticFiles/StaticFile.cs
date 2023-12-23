using Core.Enum;
using Domain.AppConfigs;
using Domain.Services;
using Domain.ServiceTypes;

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

        public List<Service> Services { get; set; }
        public List<ServiceType> ServiceTypes { get; set; }
        public List<AppConfig> AppConfigLogos { get; set; }
        
        public List<AppConfig> AppConfigIcons { get; set; }
    }
}