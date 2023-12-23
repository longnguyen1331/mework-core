using Domain.Identity.Users;
using Domain.Services;
using Domain.StaticFiles;

namespace Domain.ServiceTypes
{
    public class ServiceType
    {
        public ServiceType() 
        { 
            ODX = 0;
            IsDeleted = false;
            IsHighLight = false;
            IsVisibled = false;
        }

        public Guid Id { get; set;}
        public string? ImageUrl { get; set; }
        public int ODX { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsHighLight { get; set; }
        public bool IsVisibled { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public Guid? CreatedBy { get; set; }
        public User? CreatedUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public User? ModifiedUser { get; set; }
        public Guid? ImageId { get; set; }
        public StaticFile? ImageFile { get; set; }
        public List<Service>? Services { get; set; }

    }

}