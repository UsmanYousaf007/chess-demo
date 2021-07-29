using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace HUF.Utils.Runtime.Extensions
{
    public static class TypeExtensions
    {
        
        /// <summary>
        /// Returns an enumerator with types that derive from a given type.
        /// </summary>
        /// <param name="type">A type.</param>
        /// <param name="includeBase">Whether to include base type in the enumerator.</param>
        [PublicAPI]
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