using System.Collections.Generic;

public class UnityAdsPackageConfig : PackageConfig
{
    public override string Name
    {
        get { return "UnityAds"; }
    }

    public override string Version
    {
        get { return "1.0.1"; }
    }

    public override Dictionary<Platform, string> NetworkSdkVersions
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "3.0.0" },
                { Platform.IOS, "3.0.0" }
            };
        }
    }

    public override Dictionary<Platform, string> AdapterClassNames
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "com.mopub.mobileads.Unity" },
                { Platform.IOS, "UnityAds" }
            };
        }
    }
}
