using System;
using System.Collections.Generic;
using System.Linq;

namespace HUF.Utils.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetDerivedTypes(this Type type, bool includeBase = false)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var allTypes = assemblies.SelectMany(domainAssembly => domainAssembly.GetTypes(),
                (domainAssembly, assemblyType) => assemblyType);
            var assignableTypes = allTypes.Where(type.IsAssignableFrom);
            if (!includeBase)
                assignableTypes = assignableTypes.Where(x => x != type);
            return assignableTypes.ToArray();
        }
    }
}