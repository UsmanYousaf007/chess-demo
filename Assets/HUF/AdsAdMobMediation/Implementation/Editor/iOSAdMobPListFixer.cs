using System.Linq;
using HUF.Utils.Assets.Editor;
using HUF.Utils.BuildSupport.Editor.iOS;
using HUF.Utils.Extensions;
using JetBrains.Annotations;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace HUF.AdsAdMobMediation.Implementation.Editor
{
    [UsedImplicitly]
    public class iOSAdMobPListFixer : iOSPlistBaseFixer
    {
        const string LOG_PREFIX = nameof(iOSAdMobPListFixer);
        public override int callbackOrder => 10;
        
        protected override bool Process(PlistElementDict rootDict, string projectPath)
        {
            var configs = AssetsUtils.GetByFilter<AdMobProviderConfig>($"t: {nameof(AdMobProviderConfig)}");
            if (configs.Count == 0)
            {
                Debug.LogWarning($"[{LOG_PREFIX}] Can't find any config of type {nameof(AdMobProviderConfig)} " +
                                 "in your project");
                return false;
            }
            
            if (configs.Count > 1)
                Debug.LogWarning($"[{LOG_PREFIX}] There is more than one {nameof(AdMobProviderConfig)} in your project");
            
            var config = configs.First();
            
            if(config.AppLovinSdkKey.IsNotEmpty())
                rootDict.SetString("AppLovinSdkKey", config.AppLovinSdkKey);
            
            if(config.AppId.IsNotEmpty())
                rootDict.SetString("GADApplicationIdentifier", config.AppId);
                
            return true;
        }
    }
}
