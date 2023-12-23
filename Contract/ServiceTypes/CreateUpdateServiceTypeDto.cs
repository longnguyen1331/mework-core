using Domain.Identity.Users;
using System;

namespace Contract.ServiceTypes
{
    public class CreateUpdateServiceTypeDto
    {
        public CreateUpdateServiceTypeDto()
        {
            IsVisibled = true;
            IsDeleted = false;
            IsHighLight = false;
        }
        public string? ImageUrl { get; set; }
        public Guid? ImageId { get; set; }
        public int ODX { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsHighLight { get; set; }
        public bool IsVisibled { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}