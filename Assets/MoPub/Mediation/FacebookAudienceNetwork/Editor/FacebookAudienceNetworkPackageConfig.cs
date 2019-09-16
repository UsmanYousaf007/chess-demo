using System.Collections.Generic;

public class FacebookAudienceNetworkPackageConfig : PackageConfig
{
    public override string Name
    {
        get { return "FacebookAudienceNetwork"; }
    }

    public override string Version
    {
        get { return /*UNITY_PACKAGE_VERSION*/"1.3.7"; }
    }

    public override Dictionary<Platform, string> NetworkSdkVersions
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, /*ANDROID_SDK_VERSION*/"5.5.0" },
                { Platform.IOS, /*IOS_SDK_VERSION*/"5.5.0" }
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
