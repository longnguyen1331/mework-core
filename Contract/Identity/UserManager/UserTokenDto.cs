namespace Contract.Identity.UserManager
{
    public class UserTokenDto
    {
        public Guid UserId { get; set; }
        public UserDto User { get; set; }
        public string Value { get; set; }
    }
}