using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCms.Documents.Core
{
    public static class DocumentInitHelper
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Must be called on every init
        /// </summary>
        public static IDocumentStore InitIdsConventions(this IDocumentStore store)
        {
            store.Conventions.FindIdentityProperty = memberInfo => memberInfo.Name == "Id";


            store.Conventions.RegisterIdConvention<UserImage>((dbname, commands, userImage) => UserImage.IdPrefix + userImage.PageId + "/");
            store.Conventions.RegisterIdConvention<UserFile>((dbname, commands, userFile) => UserFile.IdPrefix + userFile.PageId + "/");
            return store;
        }        
    }
}
