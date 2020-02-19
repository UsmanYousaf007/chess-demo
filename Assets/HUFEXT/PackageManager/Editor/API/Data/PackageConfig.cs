namespace HUFEXT.PackageManager.Editor.API.Data
{
    [System.Serializable]
    public class PackageConfig
    {
        public string packageName;
        public string latestVersion;
        public string minimumVersion;
        public bool packageCanBeRemoved = true;
        public bool overwritePackage = false;
    }
}