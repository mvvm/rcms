using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCms.Common
{
    public class ImageUploadSettings
    {
        public bool StoreNormal { get; set; }
        public bool StoreSource { get; set; }
        public bool StoreThumb { get; set; }

        public int ThumbWidth { get; set; }
        public int ThumbHeight { get; set; }

        public int NormalWidth { get; set; }
        public int NormalHeight { get; set; }

        public int CropWindowWidth { get; set; }
        public int CropWindowHeight { get; set; }
    }
}
