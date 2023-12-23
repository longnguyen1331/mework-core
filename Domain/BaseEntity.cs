using Domain.Identity.Users;

namespace Domain.AppConfigs
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public Guid? CreatedBy { get; set; }
        public User? CreatedUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public User? ModifiedUser { get; set; }
    }
}