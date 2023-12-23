using Domain.Departments;
using Domain.Positions;

namespace Domain.Identity.Users
{
    public class UserWithNavigationProperties
    {

        public User User { get; set; }
        public int Count { get; set; } = 0;
        public List<string> RoleNames { get; set; }
        public Position Position { get; set; }
        public List<Department> Departments { get; set; }
    }
}