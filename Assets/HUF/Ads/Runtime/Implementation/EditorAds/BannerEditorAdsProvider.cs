using System;
using HUF.Ads.Runtime.API;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Ads.Runtime.Implementation.EditorAds
{
    public class BannerEditorAdsProvider : IBannerAdProvider
    {
        readonly HLogPrefix logPrefix = new HLogPrefix( nameof(BannerEditorAdsProvider) );
        public string ProviderId => "EditorAds";
        public bool IsInitialized => true;

#pragma warning disable CS0067
        public event Action<IBannerCallbackData, bool> OnBannerShown;
        public event Action<IBannerCallbackData> OnBannerFailed;
        public event Action<IBannerCallbackData> OnBannerClicked;
        public event Action<IBannerCallbackData> OnBannerHidden;
#pragma warning restore CS0067

        public bool Init()
        {
            HLog.Log( logPrefix, $"Initialized Editor Interstitial ads provider" );
            return true;
        }

        public void CollectSensitiveData( bool consentStatus )
        {
            HLog.Log( logPrefix, $"Collect sensitive data: {consentStatus}" );
        }

        public bool Show( BannerPosition position = BannerPosition.BottomCenter )
        {
            MockEditorBanner.Instance.Show( position );
            return true;
        }

        public bool Show( string placementId, BannerPosition position = BannerPosition.BottomCenter )
        {
            MockEditorBanner.Instance.Show( position );
            return true;
        }

        public void Hide()
        {
            MockEditorBanner.Instance.Hide();
        }
    }
}