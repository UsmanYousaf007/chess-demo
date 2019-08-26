using System.Collections.Generic;

public class AdColonyPackageConfig : PackageConfig
{
    public override string Name
    {
        get { return "AdColony"; }
    }

    public override string Version
    {
        get { return "1.0.4"; }
    }

    public override Dictionary<Platform, string> NetworkSdkVersions
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "3.3.8" },
                { Platform.IOS, "3.3.7" }
            };
        }
    }

    public override Dictionary<Platform, string> AdapterClassNames
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "com.mopub.mobileads.AdColony" },
                { Platform.IOS, "AdColony" }
            };
        }
    }
}
