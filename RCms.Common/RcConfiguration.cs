using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RCms.Common.Enums;

namespace RCms.Common
{
    public static class RcConfiguration
    {
        public static class Common
        {
            private const string Prefix = "Common.";
            public static string CdnEndpointUrl {
                get { return ConfigurationManager.AppSettings[Prefix + "CdnEndpointUrl"]; }
            }
            public static string BlobEndpointUrl {
                get { return ConfigurationManager.AppSettings[Prefix + "BlobEndpointUrl"]; }
            }

            public static string StorageConnectionString {
                get { return ConfigurationManager.AppSettings[Prefix + "StorageConnectionString"]; }
            }

            public const string StorageBlobFileContainer = "1buro-files";
            public const string StorageBlobImagesContainer = "1buro-images";
        }

        public static class Email
        {
            private const string Prefix = "Email.";

            public static bool IsEmailSendingEnabled {
                get { return bool.Parse(ConfigurationManager.AppSettings[Prefix + "IsEmailSendingEnabled"]); }
            }

            public static string UserName {
                get { return ConfigurationManager.AppSettings[Prefix + "UserName"]; }
            }

            public static string Password {
                get { return ConfigurationManager.AppSettings[Prefix + "Password"]; }
            }

            public static string FromName {
                get { return ConfigurationManager.AppSettings[Prefix + "FromName"]; }
            }

            public static string FromEmail {
                get { return ConfigurationManager.AppSettings[Prefix + "FromEmail"]; }
            }

            public static string Server {
                get { return ConfigurationManager.AppSettings[Prefix + "Server"]; }
            }

            public static int Port {
                get { return int.Parse(ConfigurationManager.AppSettings[Prefix + "Port"]); }
            }

            public static bool EnableSsl {
                get { return bool.Parse(ConfigurationManager.AppSettings[Prefix + "EnableSsl"]); }
            }
        }

        public static class ImageUpload
        {
            private static readonly Dictionary<UserImageTypes, ImageUploadSettings> _settings;

            static ImageUpload()
            {
                _settings = new Dictionary<UserImageTypes, ImageUploadSettings>
                {

                };
            }

            public static ImageUploadSettings GetSettings(UserImageTypes userImageType)
            {
                return _settings[userImageType];
            }
        }
    }

    
}
