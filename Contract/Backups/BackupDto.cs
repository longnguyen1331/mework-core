namespace Contract.Backups
{
    public class BackupDto
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string? UserFullName { get; set; } 
        public DateTime CreatedDate { set; get; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Server { get; set; }
        public string? DbName { get; set; }
        public string? UserName { get; set; }
        public string? ConnectionString { get; set; }
        public string? Password { get; set; }
        public string? FullFilePath { get; set; }
    }
}