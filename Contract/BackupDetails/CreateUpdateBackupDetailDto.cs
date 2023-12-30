namespace Contract.BackupDetails
{
    public class CreateUpdateBackupDetailDto
    {
        public Guid Id { get; set; }
        public Guid BackupId { get; set; }
        public Guid? UserId { get; set; }
        public string? FullFilePath { get; set; }
    }
}