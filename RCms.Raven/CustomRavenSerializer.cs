using Raven.Imports.Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCms.Raven
{
    public class CustomRavenSerializer : DefaultContractResolver
    {
        public CustomRavenSerializer()
            : base(true)
        {
        }

        protected override List<System.Reflection.MemberInfo> GetSerializableMembers(Type objectType)
        {
            return base.GetSerializableMembers(objectType)
                .Where(a => a.GetCustomAttributes(typeof(RavenIgnoreAttribute), true).Any() == false)
                .ToList();
        }
    }
}
