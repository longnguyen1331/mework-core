using Core.Enum;
using System.Text;

namespace Contract.Identity.UserManager
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public int Count { get; set; } = 0;
        public string UserName { get; set; }
        public string UserCode { get; set;}
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public Gender Gender { get; set; } 
        public DateTime DOB { get; set; } 
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string AvatarURL { get; set; }



        public Guid? BloodTypeId { set; get; }
        public string? CitizenIDNumber { get; set; }
        public Guid? CountryId { get; set; }
        public string? CountryName { get; set; }
        public Guid? ProvinceId { get; set; }
        public string? ProvinceName { get; set; }
        public Guid? DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public Guid? WardId { get; set; }
        public string? WardName { get; set; }
        public string? Address { get; set; }
        public Guid? DependsId { get; set; }
        public int? Relationship { get; set; }

        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public int? ODX { get; set; }
    }
}