using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCms.Raven
{
    [System.AttributeUsage(System.AttributeTargets.Property | AttributeTargets.Field)]
    public class RavenIgnoreAttribute : Attribute
    {
    }
}
