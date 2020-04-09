using System.Linq;
using HUF.AdsAdMobMediation.API;
using HUF.AdsAdMobMediation.Implementation;
using HUF.Utils.Assets.Editor;
using HUF.Utils.BuildSupport.Editor.iOS;
using HUF.Utils.Extensions;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEditor.iOS.Xcode;


namespace HUF.AdsAdMobMediation.Editor
{
    [UsedImplicitly]
    public class iOSAdMobPlistFixer : iOSPlistBaseFixer
    {
        const string ALLOW_LOCAL_NETWORKING = "NSAllowsLocalNetworking";
        const string ALLOW_WEB_CONTENT = "NSAllowsArbitraryLoadsInWebContent";
        const string ALLOW_ATRITRARY_LOADS = "NSAllowsArbitraryLoads";
        const string APP_TRANSPORT_SECURITY = "NSAppTransportSecurity";
        
        static readonly HLogPrefix logPrefix = new HLogPrefix( HAdsAdMobMediation.logPrefix, nameof(iOSAdMobPlistFixer) );
        
        public override int callbackOrder => 10;
        
        protected override bool Process(PlistElementDict rootDict, string projectPath)
        {
            var configs = AssetsUtils.GetByFilter<AdMobProviderConfig>($"t: {nameof(AdMobProviderConfig)}");
            if (configs.Count == 0)
            {
                HLog.LogWarning(logPrefix, $"Can't find any config of type {nameof(AdMobProviderConfig)} in your project");
                return false;
            }
            
            if (configs.Count > 1)
                HLog.LogWarning(logPrefix, $"There is more than one {nameof(AdMobProviderConfig)} in your project");
            
            var config = configs.First();
            
            if(config.AppLovinSdkKey.IsNotEmpty())
                rootDict.SetString("AppLovinSdkKey", config.AppLovinSdkKey);
            
            if(config.AppId.IsNotEmpty())
                rootDict.SetString("GADApplicationIdentifier", config.AppId);
                
            PlistElementDict securityDict = rootDict;
            if (!rootDict.values.ContainsKey(APP_TRANSPORT_SECURITY))
            {
                securityDict = rootDict.CreateDict(APP_TRANSPORT_SECURITY);
            }
            else
            {
                securityDict = rootDict.values[APP_TRANSPORT_SECURITY].AsDict();
            }
            
            
            if (!securityDict.values.ContainsKey(ALLOW_ATRITRARY_LOADS))
                securityDict.SetBoolean(ALLOW_ATRITRARY_LOADS, true);
            
            if (securityDict.values.ContainsKey(ALLOW_WEB_CONTENT))
            {
                securityDict.values.Remove(ALLOW_WEB_CONTENT);
            }
            
            if (securityDict.values.ContainsKey(ALLOW_LOCAL_NETWORKING))
            {
                securityDict.values.Remove(ALLOW_LOCAL_NETWORKING);
            }
            
            return true;
        }
    }
}
