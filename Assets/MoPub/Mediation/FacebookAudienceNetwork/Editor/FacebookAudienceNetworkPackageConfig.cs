using System.Collections.Generic;

public class FacebookAudienceNetworkPackageConfig : PackageConfig
{
    public override string Name
    {
        get { return "FacebookAudienceNetwork"; }
    }

    public override string Version
    {
        get { return "1.0.2"; }
    }

    public override Dictionary<Platform, string> NetworkSdkVersions
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "5.1.0" },
                { Platform.IOS, "5.1.0" }
            };
        }
    }

    public override Dictionary<Platform, string> AdapterClassNames
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "com.mopub.mobileads.FacebookAudienceNetwork" },
                { Platform.IOS, "FacebookAudienceNetwork" }
            };
        }
    }
}
