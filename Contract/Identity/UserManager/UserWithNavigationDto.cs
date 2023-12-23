using Contract.Departments;
using Contract.Positions;

namespace Contract.Identity.UserManager
{
    public class UserWithNavigationPropertiesDto
    {
        public int Index { get; set;}
        public int Count { get; set; } = 0;
        public UserDto User { get; set; } = new UserDto();
        public List<string> RoleNames { get; set; } = new List<string>();
        public PositionDto Position { get; set; } = new PositionDto();
        public List<DepartmentDto> Departments { get; set; } = new List<DepartmentDto>();
        

    }
}