using System;
using RCms.Documents.Core;

namespace RCms.Documents
{
    public class MigrationHistory: IIdentifiable
    {
        public const string IdPrefix = "migrationhistory/";
        public string Id { get; set; }
        public int Version { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public string MigrationLog { get; set; }
    }
}
