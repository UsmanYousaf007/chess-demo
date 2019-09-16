using System.Collections.Generic;

public class IronSourcePackageConfig : PackageConfig
{
    public override string Name
    {
        get { return "IronSource"; }
    }

    public override string Version
    {
        get { return "1.1.0"; }
    }

    public override Dictionary<Platform, string> NetworkSdkVersions
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "6.8.2" },
                { Platform.IOS, "6.8.1.0" }
            };
        }
    }

    public override Dictionary<Platform, string> AdapterClassNames
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "com.mopub.mobileads.IronSource" },
                { Platform.IOS, "IronSource" }
            };
        }
    }
}
