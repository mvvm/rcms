using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using RCms.Common.Extensions;

namespace RCms.Business.Services.Common
{
    public class UserService: IUserService
    {
        public string GetCurrentUserName()
        {
            return HttpContext.Current.User.Identity.Name;
        }

        public bool HaveAccess()
        {
            return HttpContext.Current.User.Identity.IsAuthenticated && HttpContext.Current.User.IsInRole("ContentManager");
        }

        public string GetCurrentUserNameBase64()
        {
            return GetCurrentUserName().Base64Encode();
        }
    }
}
