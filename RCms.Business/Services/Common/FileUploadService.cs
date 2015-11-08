using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Raven.Client;
using RCms.Common;
using RCms.Common.Extensions;
using RCms.Documents;

namespace RCms.Business.Services.Common
{
    public class FileUploadService : IFileUploadService
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        
        private readonly IUserFileService _userFileService;
        private readonly IUserService _userService;
        private readonly IDocumentSession _session;

        public FileUploadService(IUserFileService userFileService, IUserService userService, IDocumentSession session)
        {
            _userFileService = userFileService;
            _userService = userService;
            _session = session;
        }


        public CloudBlobContainer GetFileStorageContainer()
        {
            return GetStorageContainer(RcConfiguration.Common.StorageBlobFileContainer);
        }
        public CloudBlobContainer GetImageStorageContainer()
        {
            return GetStorageContainer(RcConfiguration.Common.StorageBlobImagesContainer);
        }
        private CloudBlobContainer GetStorageContainer(string container)
        {
            var connString = RcConfiguration.Common.StorageConnectionString;
            var storageAccount = CloudStorageAccount.Parse(connString);

            var client = storageAccount.CreateCloudBlobClient();
            return client.GetContainerReference(container);
        }

        /// <summary>
        /// Convert source file name - to valid Azure file name
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string ConvertBlobNameToStoreInAzure(string encodedUserId, string hash, string fileName)
        {
            return string.Format("{0}/{1}/{2}", encodedUserId, hash, fileName);
        }

        public UserFile StoreFile(HttpPostedFile file)
        {
            var userNameBase64 = _userService.GetCurrentUserId();
            byte[] fileData;
            string hash;
            // convert file to byte array and calculate hash of content
            if (ConvertFileToByteArray(file.InputStream, file.ContentLength, out fileData, out hash) == false)
            {
                return null;
            }

            // check that document already uploaded:
            var fileWithSameHashAndNameUploadedByUser = _session.Query<UserFile>().Where(x => x.Hash == hash && x.OriginalFileName == file.FileName && x.CreatedBy == userNameBase64).FirstOrDefault();
            if (fileWithSameHashAndNameUploadedByUser != null)
            {
                return fileWithSameHashAndNameUploadedByUser;
            }
            
            // upload file in Azure
            var container = GetFileStorageContainer();
            var blockBlob = container.GetBlockBlobReference(ConvertBlobNameToStoreInAzure(_userService.GetCurrentUserId(), hash, file.FileName));
            blockBlob.UploadFromByteArray(fileData, 0, fileData.Length);
            var uri = blockBlob.Uri;

            // store file in Raven
            UserFile userFile = _userFileService.Save(file.FileName, hash, file.ContentLength, uri.AbsolutePath);
            
            return userFile;
        }

        public IEnumerable<UserFile> UploadAttachments(HttpFileCollection postedFiles)
        {
            foreach (var fileKey in postedFiles.AllKeys)
            {
                var file = postedFiles[fileKey];
                var uploadedFile = StoreFile((HttpPostedFile)file);
                if (uploadedFile != null)
                {
                    yield return uploadedFile;
                }
            }
        }

        public bool ConvertFileToByteArray(Stream stream, int length, out byte[] result, out string hash)
        {
            result = null;
            hash = null;
            try
            {
                using (var binaryReader = new BinaryReader(stream))
                {
                    result = binaryReader.ReadBytes(length);

                    MD5 md5 = new MD5CryptoServiceProvider();
                    byte[] checkSum = md5.ComputeHash(result);

                    hash = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                }
                return true;
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Could not convert file into byte array");
                return false;
            }

        }
    }
}
