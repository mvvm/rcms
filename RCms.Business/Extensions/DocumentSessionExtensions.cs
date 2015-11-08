using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;
using RCms.Documents.Core;

namespace RCms.Business.Extensions
{
    public static class DocumentSessionExtensions
    {
        public static IEnumerable<T> CreateMultipleDocuments<T>(this IDocumentSession session, IEnumerable<T> documents) where T : class, IIdentifiable
        {
            if (documents == null) throw new ArgumentNullException("documents", "documents must be specified");

            foreach (var d in documents)
                session.Store(d);
            session.SaveChanges();
            return documents;
        }
    }
}
