using System;
using System.Collections.Generic;

namespace HUFEXT.PackageManager.Editor.Models
{
    [Serializable]
    public class ScopedRegistryWrapper
    {
        public string       name;
        public string       url;
        public List<string> scopes = new List<string>();
    }
}
