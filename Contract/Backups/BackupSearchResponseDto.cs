namespace Contract.Backups
{
    public class BackupSearchResponseDto
    {
        public int TotalItem { set; get; } = 0;
        public List<BackupDto> Result { set; get; } = new List<BackupDto>();
    }
}
