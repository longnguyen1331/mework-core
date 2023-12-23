using System;
using Microsoft.AspNetCore.Identity;

namespace Domain.Identity.Roles

{
    public class Role : IdentityRole<Guid>
    {
        public string? Code { get; set; }
    }
}