using System.Collections.Generic;

/***************************************************************
 * author:  john greaux
 * date:    05/17/2017
 * desc:    viewmodels for storing file and directory information
 **************************************************************/
namespace FileBrowser.Models
{
    public class BrowserViewModel
    {
        public long TotalFiles { get; set; }
        public long TotalFolders { get; set; }
        public string TotalSize { get; set; }
        public List<FileViewModel> Files { get; set; }
    }

    public class FileViewModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string LastModified { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
    }
}