using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;
using RCms.Common;
using RCms.Documents;

namespace RCms.Business.Services.Common
{
    public class UserFileService: IUserFileService
    {
        private readonly IDocumentSession _session;
        private readonly IUserService _userService;
        public UserFileService( IDocumentSession session, IUserService userService)
        {
            _session = session;
            _userService = userService;
        }


        public UserFile Save(string filename, string hash, long size, string relativePath)
        {
            var userFile = new UserFile()
            {
                OriginalFileName = filename,
                Hash = hash,
                RelativePath = relativePath,
                Size = size,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = _userService.GetCurrentUserId(),
                AbsoluteBlobPath = RcConfiguration.Common.BlobEndpointUrl + relativePath,
                AbsolutePath = RcConfiguration.Common.CdnEndpointUrl + relativePath
            };

            _session.Store(userFile);
            _session.SaveChanges();
            
            return userFile;
        }
    }
}
