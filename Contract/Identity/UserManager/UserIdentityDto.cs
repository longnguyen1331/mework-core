using System;

namespace Contract.Identity.UserManager
{
    public class UserIdentityDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string UserCode { get; set;}

    }
}