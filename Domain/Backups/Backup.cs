using Domain.Identity.Users;
namespace Domain.Backups
{
    public class Backup
    {
        public Backup() 
        {
            CreatedDate = DateTime.Now;
        }
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreatedDate { set; get; }
        public string? Server { get; set; }
        public string? DbName { get; set; }
        public string? ConnectionString { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public User? User { get; set; }
    }
}