using System;

namespace Contract.Identity.UserManager
{
    public class UpdateUserProfileRequestDto
    {
        public Guid Id { get; set; }
        public DateTime DOB { get; set; }
        public string AvatarURL { get; set;}
        
    }
}