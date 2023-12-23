using System;
using Domain.Departments;
using Domain.Identity.Users;

namespace Domain.UserDepartments
{
    public class UserDepartment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid DepartmentId { get; set;}
        
        
        
        
        public User User { get; set; }
        public Department Department { get; set; }
    }
}