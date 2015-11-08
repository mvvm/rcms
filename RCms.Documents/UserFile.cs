using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCms.Documents
{
    /// <summary>
    /// File - Uploaded by Person.
    /// 
    /// Second Key: Hash + PersonId
    /// If person upload one file (with hash) multiple - only one UserFile will created
    /// </summary>
    public sealed class UserFile
    {
        public const string IdPrefix = "UserFile/";
        public UserFile()
        {

        }

        public string OriginalFileName { get; set; }
        public string RelativePath { get; set; }
        public string Hash { get; set; }
        public long Size { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        
        public string AbsolutePath { get; set; }
        public string AbsoluteBlobPath { get; set; }
        public string PageId { get; set; }
    }
}
