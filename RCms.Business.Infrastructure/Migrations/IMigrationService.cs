using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCms.Business.Infrastructure.Migrations
{
    public interface IMigrationService
    {
        void Migrate();
    }
}
