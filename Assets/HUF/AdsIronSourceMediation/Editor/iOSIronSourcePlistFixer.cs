#if UNITY_IOS
using System.Linq;
using HUF.AdsIronSourceMediation.Runtime.API;
using HUF.AdsIronSourceMediation.Runtime.Implementation;
using HUF.Utils.BuildSupport.Editor.iOS;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEditor.Build;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace HUF.AdsIronSourceMediation.Editor
{
    [UsedImplicitly]
    public class iOSIronSourcePlistFixer : iOSPlistBaseFixer
    {
        const string ALLOW_LOCAL_NETWORKING = "NSAllowsLocalNetworking";
        const string ALLOW_WEB_CONTENT = "NSAllowsArbitraryLoadsInWebContent";
        const string ALLOW_ARBITRARY_LOADS = "NSAllowsArbitraryLoads";
        const string APP_TRANSPORT_SECURITY = "NSAppTransportSecurity";
        const string APP_LOADS_FOR_MEDIA= "NSAllowsArbitraryLoadsForMedia";
        
        
        public static readonly HLogPrefix logPrefix = new HLogPrefix( HAdsIronSourceMediation.logPrefix, nameof(iOSIronSourcePlistFixer) );
        public override int callbackOrder => 0;

        protected override bool Process(PlistElementDict rootDict, string projectPath)
        {
            var configs = Resources.LoadAll<AbstractConfig>("")
                .Select(x => x as IronSourceAdsProviderConfig)
                .Where(x => x != null).ToList();

            if (configs.Count == 0)
            {
                HLog.LogError(logPrefix, $"Can't find {nameof(IronSourceAdsProviderConfig)} config.");
                return false;
            }
            
            if(configs.Count > 1)
                HLog.LogWarning(logPrefix, $"Found more than one {nameof(IronSourceAdsProviderConfig)} config. " +
                                 "Only one instance of a config is allowed. Please check your configuration.");

           
            var config = configs.First();

            if (config.IOSAdMobKey.IsNotEmpty())
            {
                rootDict.SetString("GADApplicationIdentifier", config.IOSAdMobKey);
            }
            else
            {
                var errorLog = $"{nameof(IronSourceAdsProviderConfig)} config AdMob key is empty!";
                HLog.LogError( logPrefix,  errorLog);
                throw new BuildFailedException( errorLog );
            }
            
            PlistElementDict securityDict = rootDict;
            if (!rootDict.values.ContainsKey(APP_TRANSPORT_SECURITY))
            {
                securityDict = rootDict.CreateDict(APP_TRANSPORT_SECURITY);
            }
            else
            {
                securityDict = rootDict.values[APP_TRANSPORT_SECURITY].AsDict();
            }
            
            securityDict.SetBoolean(ALLOW_ARBITRARY_LOADS, true);
            securityDict.SetBoolean(ALLOW_WEB_CONTENT, true);
            securityDict.SetBoolean(APP_LOADS_FOR_MEDIA, true);

            if (securityDict.values.ContainsKey(ALLOW_LOCAL_NETWORKING))
            {
                securityDict.values.Remove(ALLOW_LOCAL_NETWORKING);
            }
            
            rootDict.SetString("NSCalendarsUsageDescription", "{PRODUCT_NAME} requests access to the Calendar");
            rootDict.SetString("NSPhotoLibraryUsageDescription", "{PRODUCT_NAME} requests access to the Photo Library");
            rootDict.SetString("NSPhotoLibraryAddUsageDescription", "{PRODUCT_NAME} requests write access to the Photo Library");
            rootDict.SetString("NSCameraUsageDescription", "{PRODUCT_NAME} requests write access to the Camera");

            return true;
        }
    }
}
#endif