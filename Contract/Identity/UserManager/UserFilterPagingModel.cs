using Core.Enum;

namespace Contract.Identity.UserManager
{
    public class UserFilterPagingModel : BaseFilterPagingDto
    {
        public string? FullName { get; set; }
        public string? UserCode { get; set; }
        public string? RoleName { get; set; }
        public Guid? CreatedBy { get; set; }
        public int? CompanyId { get; set; }
        public List<Guid>? DepartmentIds { get; set; }
        public List<Guid>? UserIds { get; set; }
        public List<Guid>? RoleIds { get; set; }
        public List<string>? Phones { get; set; }
        public Guid? ProvinceId { get; set; }
        public Guid? DistrictId { get; set; }
        public Gender? Gender{ get; set; }
        public DateTime? DobFrom { get; set; }
        public DateTime? DobTo { get; set; }
    }
}
