using System.Collections.Generic;

public class AppLovinPackageConfig : PackageConfig
{
    public override string Name
    {
        get { return "AppLovin"; }
    }

    public override string Version
    {
        get { return "1.1.0"; }
    }

    public override Dictionary<Platform, string> NetworkSdkVersions
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "9.2.1" },
                { Platform.IOS, "6.2.0" }
            };
        }
    }

    public override Dictionary<Platform, string> AdapterClassNames
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "com.mopub.mobileads.AppLovin" },
                { Platform.IOS, "AppLovin" }
            };
        }
    }
}
