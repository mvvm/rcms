using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;
using Raven.Client.Linq;

namespace RCms.Raven
{
    public static class DocumentSessionExtension
    {
        public const int PAGE_SIZE = 128;
        
        public static IEnumerable<T> GetAllDocuments<T>(this IRavenQueryable<T> queryable)
        {
            int i = 0;
            do
            {
                var results = queryable
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                    .Skip(i * PAGE_SIZE)
                    .Take(PAGE_SIZE)
                    .ToList();

                if (results.Any() == false)
                {
                    break;
                }

                foreach (var item in results)
                {
                    yield return item;
                }

                i++;
            } while (true);

        }

        public static IEnumerable<T> GetAllDocuments<T>(this IDocumentSession session)
        {
            return GetAllDocuments<T>(session.Query<T>());
        }
    }
}
