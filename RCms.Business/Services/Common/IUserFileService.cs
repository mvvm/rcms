using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RCms.Documents;

namespace RCms.Business.Services.Common
{
    public interface IUserFileService
    {
        UserFile Save(string filename, string hash, long size, string relativePath);
    }
}
