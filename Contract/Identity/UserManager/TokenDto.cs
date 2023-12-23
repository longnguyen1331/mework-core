namespace Contract.Identity.UserManager
{
    public class TokenDto
    {
        public string? UserId { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}