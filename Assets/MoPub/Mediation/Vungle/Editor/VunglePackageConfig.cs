using System.Collections.Generic;

public class VunglePackageConfig : PackageConfig
{
    public override string Name
    {
        get { return "Vungle"; }
    }

    public override string Version
    {
        get { return "1.0.1"; }
    }

    public override Dictionary<Platform, string> NetworkSdkVersions
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "6.3.24" },
                { Platform.IOS, "6.3.2" }
            };
        }
    }

    public override Dictionary<Platform, string> AdapterClassNames
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "com.mopub.mobileads.Vungle" },
                { Platform.IOS, "Vungle" }
            };
        }
    }
}
