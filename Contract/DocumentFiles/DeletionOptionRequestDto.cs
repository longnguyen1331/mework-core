using System;

namespace Contract.DocumentFiles
{
    public class SelectedDeletionRequestDto
    {
        public Guid FileId { get; set; }
        public FileDeletionOption Option { get; set; } = FileDeletionOption.File;
    }
    
    public enum FileDeletionOption{
        File,
        Revoke
    }
}