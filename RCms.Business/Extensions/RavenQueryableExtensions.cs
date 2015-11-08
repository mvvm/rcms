using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Linq;

namespace RCms.Business.Extensions
{
    public static class RavenQueryableExtensions
    {
        public static readonly TimeSpan _resultsAsOf = TimeSpan.FromSeconds(-5); //TODO: Make this config
        public static DateTime cutOff { get { return DateTime.UtcNow.Add(_resultsAsOf); } }

        public static IRavenQueryable<T> DefaultCutoff<T>(this IRavenQueryable<T> source)
        {
            return source.Customize(a => a.WaitForNonStaleResultsAsOf(cutOff));
        }
    }
}
