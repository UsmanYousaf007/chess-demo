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
        public List<string> previewVersions = new List<string>();
        public List<string> stableVersions = new List<string>();
        public List<string> experimentalVersions = new List<string>();
    }
}