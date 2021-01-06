using System;
using System.Collections.Generic;

namespace HUFEXT.PackageManager.Editor.Models
{
    [System.Serializable]
    public class PackageConfig
    {
        public string packageName;
        public string latestVersion;
        public string minimumVersion;
        public bool packageCanBeRemoved = true;
        public bool overwritePackage = false;
        public List<Models.Version> stableVersions = new List<Models.Version>();
        public List<Models.Version> previewVersions = new List<Models.Version>();
        public List<Models.Version> experimentalVersions = new List<Models.Version>();

        public List<Models.Version> GetVersionsForChannel( Models.PackageChannel channel )
        {
            switch ( channel )
            {
                case Models.PackageChannel.Stable:
                    return stableVersions;
                case Models.PackageChannel.Preview:
                    return previewVersions;
                case Models.PackageChannel.Experimental:
                    return experimentalVersions;
            }

            return stableVersions;
        }
    }
}