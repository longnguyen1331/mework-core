namespace Contract.BackupDetails
{
    public class BackupDetailDto
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string? UserFullName { get; set; } 
        public string? DbName { get; set; } 
        public string? Server { get; set; }
        public DateTime CreatedDate { set; get; }
        public string? FullFilePath { get; set; }
    }
}