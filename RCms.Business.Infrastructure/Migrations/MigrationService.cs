using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NLog;
using Raven.Client;
using Raven.Client.Exceptions;
using RCms.Documents;

namespace RCms.Business.Infrastructure.Migrations
{
    public class MigrationService : IMigrationService
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IDocumentStore _store;
        public MigrationService(IDocumentStore store)
        {
            _store = store;
        }

        // using when removed old Migrations from list
        private const int VersionOffset = 0;

        /// <summary>
        /// new migrations should be added in end of this list
        /// </summary>
        private readonly List<MigrationBase> _migrations = new List<MigrationBase>()
        {
            //new Migration201504100000()
        };

        public void Migrate()
        {
            var maxVersion = VersionOffset + _migrations.Count - 1;
            if (maxVersion < 0)
            {
                return;
            }

            using (var session = _store.OpenSession())
            {
                var hasLastMigration =
                    session.Load<MigrationHistory>(MigrationHistory.IdPrefix + maxVersion.ToString()) != null;
                if (hasLastMigration)
                {
                    // do not need migrate if last migrations executed
                    _logger.Info("Migration is up to version " + maxVersion.ToString());
                    return;
                }
                var toVersion = VersionOffset + _migrations.Count;

                var existMigrations = session.Query<MigrationHistory>()
                    .Where(x => x.Version >= VersionOffset && x.Version < toVersion)
                    .OrderBy(x => x.Version)
                    .Take(1024)
                    .ToList();

                for (var index = 0; index < _migrations.Count; index++)
                {
                    if (existMigrations.Any(x => x.Version == index + VersionOffset))
                    {
                        continue;
                    }

                    try
                    {
                        var version = VersionOffset + index;
                        var migration = new MigrationHistory()
                        {
                            Id = MigrationHistory.IdPrefix + version,
                            Version = version,
                            StartDate = DateTime.UtcNow
                        };
                        session.Store(migration);
                        session.SaveChanges();
                        var migrationName = _migrations[index].GetType().Name;
                        _logger.Info("Running Migration " + migrationName);

                        var migrationLog = _migrations[index].Migrate();
                        migration = session.Load<MigrationHistory>(MigrationHistory.IdPrefix + version);
                        migration.FinishDate = DateTime.UtcNow;
                        migration.MigrationLog = migrationLog;
                        session.SaveChanges();
                        _logger.Info("Finished Migration " + migrationName);
                    }
                    catch (Exception exception)
                    {
                        _logger.Error(
                            () => string.Format("Error Migrate [{1}]: {0}. Process Stopped.", exception, index));
                        return;
                    }
                    _logger.Info("Finished Migrations");
                }
            }
        }
    }
}
