using System;
using System.Collections.Generic;
using Domain.Identity.Users;

namespace Domain.Positions
{
    public class Position
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; }
        public string? Note { get; set; }
        public int ODX { get; set; }
        
        
        
        //navigation
        public List<User> Users { get; set; }
    }
}