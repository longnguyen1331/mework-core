using Core.Enum;

namespace Website.Models
{
    public class UpdateUserModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Gender Gender { get; set; } = Gender.Male;
        public string DOB { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? CitizenIDNumber { get; set; }
        public Guid? CountryId { get; set; }
        public Guid? ProvinceId { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid? WardId { get; set; }
        public string? Address { get; set; }
        public string? AvatarURL { get; set; }
        public bool IsOnLyUpdateAvatar { get; set; } = false;
    }
}
