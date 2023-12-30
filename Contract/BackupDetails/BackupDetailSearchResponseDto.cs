namespace Contract.BackupDetails
{
    public class BackupDetailsearchResponseDto
    {
        public int TotalItem { set; get; } = 0;
        public List<BackupDetailDto> Result { set; get; } = new List<BackupDetailDto>();
    }
}
