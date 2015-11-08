using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;
using RCms.Common;
using RCms.Common.Enums;
using RCms.Documents;
using System.Drawing.Imaging;
using System.Security.Authentication;
using System.Web.Helpers;
using Microsoft.WindowsAzure.Storage.Blob;
using RCms.Business.Domain;
using RCms.Common.Helpers;
using RCms.Documents.Core;

namespace RCms.Business.Services.Common
{
    public class ImageUploadService : IImageUploadService
    {
        private readonly IUserService _userService;

        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IDocumentSession _session;
        private readonly IFileUploadService _fileUploadService;

        public ImageUploadService(IUserService userService, IDocumentSession session, IFileUploadService fileUploadService)
        {
            _userService = userService;
            _session = session;
            _fileUploadService = fileUploadService;
        }

        public byte[] ImageToByte(Image img)
        {
            var converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }


        public Stream ToStream(Image image, ImageFormat formaw)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, formaw);
            stream.Position = 0;
            return stream;
        }

        public bool SaveImage(CloudBlobContainer container, string userId, UserImageTypes userImageType, string sourceHash, string suffix, Image image, ImageFormat format, string sourceFormat, out Uri uri)
        {
            uri = null;
            try
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(string.Format("{0}-{1}-{2}-{3}.{4}", userId.Replace('/', '-'), (int)userImageType, sourceHash, suffix, sourceFormat));
                blockBlob.UploadFromStream(ToStream(image, format));
                uri = blockBlob.Uri;
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Error when save on Azure");
                return false;
            }

            return true;
        }

        public bool SaveImage(CloudBlobContainer container, string userId, UserImageTypes userImageType, string sourceHash, string suffix, WebImage image, ImageFormat format, string sourceFormat, out Uri uri)
        {
            uri = null;
            try
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(string.Format("{0}-{1}-{2}-{3}.{4}", userId.Replace('/', '-'), (int)userImageType, sourceHash, suffix, sourceFormat));
                var bytes = image.GetBytes();
                blockBlob.UploadFromByteArray(bytes, 0, bytes.Length);
                uri = blockBlob.Uri;
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Error when save on Azure");
                return false;
            }

            return true;
        }

        public IImageContainer UploadImage(UploadImageModel model, System.Web.HttpPostedFile postedFile)
        {
            var user = _userService.GetCurrentUserNameBase64();
            var userImage = new UserImage()
            {
                UserImageType = model.UserImageType,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = user
            };

            var uploadSettings = RcConfiguration.ImageUpload.GetSettings(model.UserImageType);
            byte[] fileData;
            string hash;

            if (_fileUploadService.ConvertFileToByteArray(postedFile.InputStream, postedFile.ContentLength, out fileData,
                out hash) == false)
            {
                return null;
            }

            try
            {
                var sourceImage = new WebImage(fileData);
                var imageFormat = WebImageHelper.ResolveImageFormat(sourceImage.ImageFormat);

                userImage.SourceHash = hash;
                userImage.SourceHeight = sourceImage.Height;
                userImage.SourceWidth = sourceImage.Width;

                var container = _fileUploadService.GetImageStorageContainer();

                if (uploadSettings.StoreSource)
                {
                    Uri sourceUri = null;
                    if (SaveImage(container, user, model.UserImageType, hash, "source", sourceImage, imageFormat, sourceImage.ImageFormat, out sourceUri) == false)
                    {
                        return null;
                    }
                    userImage.SourceUrl = RcConfiguration.Common.CdnEndpointUrl + sourceUri.AbsolutePath;
                }


                if (uploadSettings.StoreNormal)
                {
                    var normalImage = sourceImage.ResizeByMaxDimensionsWithStretch(uploadSettings.NormalWidth, uploadSettings.NormalHeight);

                    userImage.NormalHeight = normalImage.Height;
                    userImage.NormalWidth = normalImage.Width;

                    Uri normalUri = null;
                    if (SaveImage(container, user, model.UserImageType, hash, "page", normalImage, imageFormat, sourceImage.ImageFormat, out normalUri) == false)
                    {
                        return null;
                    }
                    userImage.NormalUrl = RcConfiguration.Common.CdnEndpointUrl + normalUri.AbsolutePath;
                }

                if (uploadSettings.StoreThumb)
                {
                    var thumbImage = sourceImage.CropAndResize(uploadSettings.ThumbWidth, uploadSettings.ThumbHeight);

                    userImage.ThumbHeight = thumbImage.Height;
                    userImage.ThumbWidth = thumbImage.Width;

                    Uri thumbUri = null;
                    if (SaveImage(container, user, model.UserImageType, hash, "thumb", thumbImage, imageFormat, sourceImage.ImageFormat, out thumbUri) == false)
                    {
                        return null;
                    }
                    userImage.ThumbUrl = RcConfiguration.Common.CdnEndpointUrl + thumbUri.AbsolutePath;
                }

                _session.Store(userImage);

                // save image to an object
                switch (model.UserImageType)
                {
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Error in processing image");
                return null;
            }
        }

        public MemoryStream DownloadBlob(CloudBlockBlob blob)
        {
            var memorystream = new MemoryStream { Position = 0 };
            blob.DownloadToStream(memorystream);
            return memorystream;
        }
    }
}
