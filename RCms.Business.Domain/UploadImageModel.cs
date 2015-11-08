using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RCms.Common;
using RCms.Common.Enums;

namespace RCms.Business.Domain
{
    public class UploadImageModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public UserImageTypes UserImageType { get; set; }
        public string DestId { get; set; }
    }
}
