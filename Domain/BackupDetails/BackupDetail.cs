using Domain.Identity.Users;
namespace Domain.BackupDetails
{
    public class BackupDetail
    {
        public BackupDetail() 
        {
            CreatedDate = DateTime.Now;
        }
        public Guid Id { get; set; }
        public Guid? BackupId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? FullFilePath { get; set; }
        public User? User { get; set; }
    }
}