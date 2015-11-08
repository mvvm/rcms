using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RCms.Common;
using RCms.Common.Enums;

namespace RCms.Documents
{
    public class UserImage
    {
        public const string IdPrefix = "UserImage/";

        public string Id { get; set; }

        public UserImageTypes UserImageType { get; set; }

        public string SourceHash { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public string SourceUrl { get; set; }
        public int SourceWidth { get; set; }
        public int SourceHeight { get; set; }


        public string ThumbUrl { get; set; }
        public int ThumbWidth { get; set; }
        public int ThumbHeight { get; set; }


        public string NormalUrl { get; set; }
        public int NormalWidth { get; set; }
        public int NormalHeight { get; set; }
        public string PageId { get; set; }
    }
}
