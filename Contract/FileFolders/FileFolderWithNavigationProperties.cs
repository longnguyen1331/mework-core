using Domain.FileFolders;

namespace Contract.FileFolders
{
    public class FileFolderWithNavigationProperties
    {
        public FileFolder FileFolder { get; set;}
        public int FileCount { get; set; }
    }
}