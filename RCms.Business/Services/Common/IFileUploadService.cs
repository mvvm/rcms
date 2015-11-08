using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Blob;
using RCms.Documents;

namespace RCms.Business.Services.Common
{
    public interface IFileUploadService
    {
        bool ConvertFileToByteArray(Stream stream, int length, out byte[] result, out string hash);
        CloudBlobContainer GetImageStorageContainer();
        CloudBlobContainer GetFileStorageContainer();
        IEnumerable<UserFile> UploadAttachments(HttpFileCollection postedFiles);
        UserFile StoreFile(HttpPostedFile file);
    }
}
