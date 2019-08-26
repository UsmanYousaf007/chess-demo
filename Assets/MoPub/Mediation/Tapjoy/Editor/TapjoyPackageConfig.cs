using System.Collections.Generic;

public class TapjoyPackageConfig : PackageConfig
{
    public override string Name
    {
        get { return "Tapjoy"; }
    }

    public override string Version
    {
        get { return "1.0.2"; }
    }

    public override Dictionary<Platform, string> NetworkSdkVersions
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "12.2.0" },
                { Platform.IOS, "12.2.0" }
            };
        }
    }

    public override Dictionary<Platform, string> AdapterClassNames
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "com.mopub.mobileads.Tapjoy" },
                { Platform.IOS, "Tapjoy" }
            };
        }
    }
}
