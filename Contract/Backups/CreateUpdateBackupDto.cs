using System;

namespace Contract.Backups
{
    public class CreateUpdateBackupDto
    {
        public Guid? Id { get; set; }
        public Guid? UserId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Server { get; set; }
        public string? DbName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? FullFilePath { get; set; }
    }
}