using System;
using System.Collections.Generic;
using Domain.UserDepartments;

namespace Domain.Departments
{
    public class Department
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; }
        public string? Note { get; set; }
        public int ODX { get; set; }
        public Guid? ParentCode { get; set; }
        
        
        //naviagation

        public List<UserDepartment> UserDepartments { get; set; }
        
    }
}