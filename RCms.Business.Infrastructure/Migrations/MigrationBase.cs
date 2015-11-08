using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCms.Business.Infrastructure.Migrations
{
    public abstract class MigrationBase
    {
        protected static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public abstract string Migrate();
    }
}
