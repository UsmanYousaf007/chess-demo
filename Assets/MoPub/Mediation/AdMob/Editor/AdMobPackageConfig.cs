using System.Collections.Generic;

public class AdMobPackageConfig : PackageConfig
{
    public override string Name
    {
        get { return "AdMob"; }
    }

    public override string Version
    {
        get { return "1.0.2"; }
    }

    public override Dictionary<Platform, string> NetworkSdkVersions
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "17.1.2" },
                { Platform.IOS, "7.39.0" }
            };
        }
    }

    public override Dictionary<Platform, string> AdapterClassNames
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "com.mopub.mobileads.GooglePlayServices" },
                { Platform.IOS, "MPGoogleAdMob" }
            };
        }
    }
}
