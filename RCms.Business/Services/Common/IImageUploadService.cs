using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Helpers;
using Microsoft.WindowsAzure.Storage.Blob;
using RCms.Business.Domain;
using RCms.Common;
using RCms.Common.Enums;
using RCms.Documents;
using RCms.Documents.Core;

namespace RCms.Business.Services.Common
{
    public interface IImageUploadService
    {
        MemoryStream DownloadBlob(CloudBlockBlob blob);
        byte[] ImageToByte(Image img);
        bool SaveImage(CloudBlobContainer container, string userId, UserImageTypes userImageType, string sourceHash, string suffix, WebImage image, ImageFormat format, string sourceFormat, out Uri uri);
        bool SaveImage(CloudBlobContainer container, string userId, UserImageTypes userImageType, string sourceHash, string suffix, Image image, ImageFormat format, string sourceFormat, out Uri uri);
        Stream ToStream(Image image, ImageFormat formaw);
        IImageContainer UploadImage(UploadImageModel model, HttpPostedFile postedFile);
    }
}